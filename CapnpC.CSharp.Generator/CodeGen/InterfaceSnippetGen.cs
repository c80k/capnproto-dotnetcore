using CapnpC.CSharp.Generator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CapnpC.CSharp.Generator.CodeGen.SyntaxHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace CapnpC.CSharp.Generator.CodeGen
{
    class InterfaceSnippetGen
    {
        readonly GenNames _names;

        public InterfaceSnippetGen(GenNames names)
        {
            _names = names;
        }

        TypeSyntax TransformReturnType(Method method)
        {
            switch (method.Results.Count)
            {
                case 0:
                    return IdentifierName(nameof(Task));

                case 1 when method.Results[0].Type.Tag == TypeTag.Struct:
                    return GenericName(nameof(Task)).AddTypeArgumentListArguments(
                        _names.MakeTypeSyntax(method.Results[0].Type, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NonNullable));

                case 1:
                    return GenericName(nameof(Task)).AddTypeArgumentListArguments(
                        _names.MakeTypeSyntax(method.Results[0].Type, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NullableRef));

                default:
                    return GenericName(nameof(Task)).AddTypeArgumentListArguments(
                        TupleType(SeparatedList(
                            method.Results.Select(
                                f => TupleElement(_names.MakeTypeSyntax(f.Type, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NullableRef))))));
            }
        }

        ParameterSyntax[] TransformParameters(Method method)
        {
            var list = new List<ParameterSyntax>();

            if (method.Params.Count > 0)
            {
                var arg0 = method.Params[0];

                if (arg0.Name == null)
                {
                    list.Add(Parameter(_names.AnonymousParameter.Identifier)
                        .WithType(_names.MakeTypeSyntax(arg0.Type, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NullableRef)));
                }
                else
                {
                    foreach (var arg in method.Params)
                    {
                        list.Add(Parameter(_names.GetCodeIdentifier(arg).Identifier)
                            .WithType(_names.MakeTypeSyntax(arg.Type, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NullableRef)));
                    }
                }
            }

            list.Add(Parameter(_names.CancellationTokenParameter.Identifier)
                .WithType(IdentifierName(nameof(CancellationToken)))
                .WithDefault(EqualsValueClause(LiteralExpression(
                    SyntaxKind.DefaultLiteralExpression,
                    Token(SyntaxKind.DefaultKeyword)))));

            return list.ToArray();
        }

        IEnumerable<TypeParameterSyntax> MakeTypeParameters(TypeDefinition def)
        {
            foreach (string name in def.GenericParameters)
            {
                yield return TypeParameter(_names.GetGenericTypeParameter(name).Identifier);
            }
        }

        IEnumerable<TypeParameterConstraintClauseSyntax> MakeTypeParameterConstraints(TypeDefinition def)
        {
            foreach (string name in def.GenericParameters)
            {
                yield return TypeParameterConstraintClause(
                    _names.GetGenericTypeParameter(name).IdentifierName)
                        .AddConstraints(ClassOrStructConstraint(SyntaxKind.ClassConstraint));
            }
        }

        public MemberDeclarationSyntax MakeInterface(TypeDefinition type)
        {
            var ifaceDecl = InterfaceDeclaration(_names.MakeTypeName(type, NameUsage.Interface).Identifier)
                .AddModifiers(Public)
                .AddAttributeLists(
                    AttributeList()
                        .AddAttributes(
                            CommonSnippetGen.MakeTypeIdAttribute(type.Id),
                            Attribute(IdentifierName("Proxy"))
                                .AddArgumentListArguments(
                                    AttributeArgument(
                                        TypeOfExpression(_names.MakeGenericTypeNameForAttribute(type, NameUsage.Proxy)))),
                            Attribute(IdentifierName("Skeleton"))
                                .AddArgumentListArguments(
                                    AttributeArgument(
                                        TypeOfExpression(_names.MakeGenericTypeNameForAttribute(type, NameUsage.Skeleton))))));

            if (type.GenericParameters.Count > 0)
            {
                ifaceDecl = ifaceDecl
                    .AddTypeParameterListParameters(MakeTypeParameters(type).ToArray())
                    .AddConstraintClauses(MakeTypeParameterConstraints(type).ToArray());
            }

            if (type.Superclasses.Count == 0)
            {
                ifaceDecl = ifaceDecl.AddBaseListTypes(SimpleBaseType(IdentifierName(nameof(IDisposable))));
            }
            else
            {
                foreach (var superClass in type.Superclasses)
                {
                    ifaceDecl = ifaceDecl.AddBaseListTypes(
                        SimpleBaseType(_names.MakeTypeSyntax(
                            superClass, type,
                            TypeUsage.DomainClass,
                            Nullability.NonNullable)));
                }
            }

            foreach (var method in type.Methods)
            {
                var methodDecl = MethodDeclaration(
                    TransformReturnType(method),
                    _names.GetCodeIdentifier(method).Identifier)
                    .AddParameterListParameters(TransformParameters(method))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                if (method.GenericParameters.Count > 0)
                {
                    methodDecl = methodDecl
                        .AddTypeParameterListParameters(MakeTypeParameters(method).ToArray())
                        .AddConstraintClauses(MakeTypeParameterConstraints(method).ToArray());

                }

                ifaceDecl = ifaceDecl.AddMembers(methodDecl);
            }

            return ifaceDecl;
        }

        bool IsSubjectToPipelining(Model.Type type, HashSet<Model.Type> visited)
        {
            if (!visited.Add(type))
                return false;

            switch (type.Tag)
            {
                case TypeTag.AnyPointer:
                case TypeTag.CapabilityPointer:
                case TypeTag.Interface:
                case TypeTag.ListPointer:
                case TypeTag.StructPointer:
                    return true;

                case TypeTag.List:
                    return IsSubjectToPipelining(type.ElementType, visited);

                case TypeTag.Struct:
                    return type.Fields.Any(f => IsSubjectToPipelining(f.Type, visited));

                default:
                    return false;
            }
        }

        bool IsSubjectToPipelining(Method method)
        {
            return method.Results.Any(r => IsSubjectToPipelining(r.Type, new HashSet<Model.Type>()));
        }

        IEnumerable<ExpressionSyntax> MakeProxyCallInitializerAssignments(Method method)
        {
            for (int i = 0; i < method.Params.Count; i++)
            {
                var methodParam = method.Params[i];
                var field = method.ParamsStruct.Fields[i];

                yield return AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    _names.GetCodeIdentifier(field).IdentifierName,
                    _names.GetCodeIdentifier(methodParam).IdentifierName);
            }
        }

        IEnumerable<ArgumentSyntax> MakeProxyReturnResultTupleElements(Method method)
        {
            foreach (var item in method.ResultStruct.Fields)
            {
                yield return Argument(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    _names.ResultLocal.IdentifierName,
                    _names.GetCodeIdentifier(item).IdentifierName));
            }
        }

        StatementSyntax MakeProxyReturnResult(Method method)
        {
            if (method.ResultStruct.Definition.SpecialName == SpecialName.MethodResultStruct)
            {
                if (method.ResultStruct.Fields.Count == 0)
                {
                    return ReturnStatement();
                }
                else
                {
                    return ReturnStatement(TupleExpression()
                        .AddArguments(MakeProxyReturnResultTupleElements(method).ToArray()));
                }

            }
            else
            {
                return ReturnStatement(_names.ResultLocal.IdentifierName);
            }
        }

        StatementSyntax MakeProxyCreateResult(Method method)
        {
            var resultType = method.ResultStruct;
            var domainType = _names.MakeTypeSyntax(resultType, method.DeclaringInterface, TypeUsage.DomainClass, Nullability.NonNullable);

            ExpressionSyntax createDomain = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(nameof(Capnp.CapnpSerializable)),
                        GenericName(nameof(Capnp.CapnpSerializable.Create))
                            .AddTypeArgumentListArguments(MakeNonNullableType(domainType))))
                        .AddArgumentListArguments(
                            Argument(_names.DeserializerLocal.IdentifierName));

            if (_names.NullableEnable)
            {
                createDomain = PostfixUnaryExpression(
                    SyntaxKind.SuppressNullableWarningExpression,
                    createDomain);
            }

            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                _names.ResultLocal.Identifier)
                                .WithInitializer(
                                        EqualsValueClause(createDomain)))));
        }

        IEnumerable<TypeParameterSyntax> MakeTypeParameters(Method method)
        {
            foreach (string name in method.GenericParameters)
            {
                yield return TypeParameter(_names.GetGenericTypeParameter(name).Identifier);
            }
        }

        IEnumerable<TypeParameterConstraintClauseSyntax> MakeTypeParameterConstraints(Method method)
        {
            foreach (string name in method.GenericParameters)
            {
                yield return TypeParameterConstraintClause(
                    _names.GetGenericTypeParameter(name).IdentifierName)
                        .AddConstraints(ClassOrStructConstraint(SyntaxKind.ClassConstraint));
            }
        }

        public MemberDeclarationSyntax MakeProxy(TypeDefinition type)
        {
            var classDecl = ClassDeclaration(_names.MakeTypeName(type, NameUsage.Proxy).Identifier)
                .AddModifiers(Public)
                .AddBaseListTypes(
                    SimpleBaseType(_names.Type<Capnp.Rpc.Proxy>(Nullability.NonNullable)),
                    SimpleBaseType(_names.MakeGenericTypeName(type, NameUsage.Interface)));

            if (type.GenericParameters.Count > 0)
            {
                classDecl = classDecl
                    .AddTypeParameterListParameters(MakeTypeParameters(type).ToArray())
                    .AddConstraintClauses(MakeTypeParameterConstraints(type).ToArray());
            }

            var allMethods =
                from c in Types.FromDefinition(type).AllImplementedClasses
                from m in c.Methods
                select m;

            foreach (var method in allMethods)
            {
                var bodyStmts = new List<StatementSyntax>();

                bodyStmts.Add(LocalDeclarationStatement(
                            VariableDeclaration(
                                IdentifierName("var"))
                            .WithVariables(
                                SingletonSeparatedList(
                                    VariableDeclarator(
                                        _names.ParamsLocal.Identifier)
                                    .WithInitializer(
                                        EqualsValueClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(nameof(Capnp.SerializerState)),
                                                    GenericName(
                                                        Identifier(nameof(Capnp.SerializerState.CreateForRpc)))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList(
                                                                _names.MakeTypeSyntax(
                                                                    method.ParamsStruct, 
                                                                    method.ParamsStruct.Definition, 
                                                                    TypeUsage.Writer, Nullability.NonNullable))))))))))));

                if (method.ParamsStruct.Definition.SpecialName == SpecialName.MethodParamsStruct)
                {
                    bodyStmts.Add(LocalDeclarationStatement(
                                VariableDeclaration(
                                    IdentifierName("var"))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            _names.AnonymousParameter.Identifier)
                                        .WithInitializer(
                                            EqualsValueClause(
                                                ObjectCreationExpression(
                                                    _names.MakeTypeSyntax(
                                                        method.ParamsStruct,
                                                        method.ParamsStruct.Definition,
                                                        TypeUsage.DomainClass,
                                                        Nullability.NonNullable))
                                                .WithArgumentList(
                                                    ArgumentList())
                                                .WithInitializer(
                                                    InitializerExpression(
                                                        SyntaxKind.ObjectInitializerExpression,
                                                        SeparatedList<ExpressionSyntax>(
                                                            CommonSnippetGen.MakeCommaSeparatedList(
                                                                MakeProxyCallInitializerAssignments(method)).ToArray())))))))));
                }

                bodyStmts.Add(ExpressionStatement(
                    ConditionalAccessExpression(
                        _names.AnonymousParameter.IdentifierName,
                        InvocationExpression(
                            MemberBindingExpression(_names.SerializeMethod.IdentifierName))
                        .AddArgumentListArguments(
                            Argument(_names.ParamsLocal.IdentifierName)))));

                var call = InvocationExpression(IdentifierName(nameof(Capnp.Rpc.BareProxy.Call)))
                    .AddArgumentListArguments(
                        Argument(
                            LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                Literal(method.DeclaringInterface.Id))),
                        Argument(
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(method.Id))),
                        Argument(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    _names.ParamsLocal.IdentifierName,
                                    GenericName(nameof(Capnp.SerializerState.Rewrap))
                                        .AddTypeArgumentListArguments(_names.Type<Capnp.DynamicSerializerState>(Nullability.NonNullable))))
                                        .AddArgumentListArguments()),
                        Argument(
                            LiteralExpression(SyntaxKind.FalseLiteralExpression)),
                        Argument(
                            _names.CancellationTokenParameter.IdentifierName));

                MethodDeclarationSyntax methodDecl;

                if (IsSubjectToPipelining(method))
                {
                    methodDecl = MethodDeclaration(
                        TransformReturnType(method),
                        _names.GetCodeIdentifier(method).Identifier)
                        .AddParameterListParameters(TransformParameters(method))
                        .AddModifiers(Public);

                    var pipelineAwareCall = InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(nameof(Capnp.Rpc.Impatient)),
                            IdentifierName(nameof(Capnp.Rpc.Impatient.MakePipelineAware))))
                            .AddArgumentListArguments(
                                Argument(call),
                                Argument(SimpleLambdaExpression(
                                    Parameter(_names.DeserializerLocal.Identifier),
                                    Block(
                                        UsingStatement(
                                            Block(  
                                                MakeProxyCreateResult(method), 
                                                MakeProxyReturnResult(method)))
                                        .WithExpression(_names.DeserializerLocal.IdentifierName)))));

                    bodyStmts.Add(ReturnStatement(pipelineAwareCall));
                }
                else
                {
                    methodDecl = MethodDeclaration(
                        TransformReturnType(method),
                        _names.GetCodeIdentifier(method).Identifier)
                        .AddParameterListParameters(TransformParameters(method))
                        .AddModifiers(Public, Token(SyntaxKind.AsyncKeyword));

                    var whenReturned = MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        call,
                        IdentifierName(nameof(Capnp.Rpc.IPromisedAnswer.WhenReturned)));

                    bodyStmts.Add(UsingStatement(
                        Block(
                            MakeProxyCreateResult(method),
                            MakeProxyReturnResult(method)))
                        .WithDeclaration(VariableDeclaration(
                            IdentifierName("var"))
                                .AddVariables(
                                    VariableDeclarator(
                                        _names.DeserializerLocal.Identifier)
                                    .WithInitializer(
                                        EqualsValueClause(
                                            AwaitExpression(whenReturned))))));
                }

                if (method.GenericParameters.Count > 0)
                {
                    methodDecl = methodDecl
                        .AddTypeParameterListParameters(MakeTypeParameters(method).ToArray())
                        .AddConstraintClauses(MakeTypeParameterConstraints(method).ToArray());
                }

                methodDecl = methodDecl.AddBodyStatements(bodyStmts.ToArray());

                classDecl = classDecl.AddMembers(methodDecl);
            }

            return classDecl;
        }

        IEnumerable<ArgumentSyntax> MakeSkeletonSetMethodTableArguments(TypeDefinition def)
        {
            foreach (var method in def.Methods)
            {
                if (method.GenericParameters.Count > 0)
                {
                    yield return Argument(
                        GenericName(_names.GetCodeIdentifier(method).ToString())
                            .AddTypeArgumentListArguments(
                                Enumerable.Repeat(
                                    _names.Type<Capnp.AnyPointer>(Nullability.NonNullable),
                                    method.GenericParameters.Count).ToArray()));
                }
                else
                {
                    yield return Argument(_names.GetCodeIdentifier(method).IdentifierName);
                }
            }
        }

        IEnumerable<ExpressionSyntax> MakeSkeletonMethodResultStructInitializer(Method method)
        {
            foreach (var arg in method.Results)
            {
                yield return AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    _names.GetCodeIdentifier(arg).IdentifierName,
                    IdentifierName(IdentifierRenamer.ToNonKeyword(arg.Name)));
            }
        }

        IEnumerable<ArgumentSyntax> MakeSkeletonMethodCallArgs(Method method)
        {
            foreach (var arg in method.ParamsStruct.Fields)
            {
                yield return Argument(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        _names.ParamsLocal.IdentifierName,
                        _names.GetCodeIdentifier(arg).IdentifierName));
            }
        }

        StatementSyntax MakeSkeletonMethodSerializerLocalDeclaration(Method method)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName("var"))
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(
                            _names.SerializerLocal.Identifier)
                        .WithInitializer(
                            EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(nameof(Capnp.SerializerState)),
                                        GenericName(
                                            Identifier(nameof(Capnp.SerializerState.CreateForRpc)))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList(
                                                    _names.MakeTypeSyntax(
                                                        method.ResultStruct,
                                                        method.ResultStruct.Definition,
                                                        TypeUsage.Writer, Nullability.NonNullable)))))))))));

        }

        CSharpSyntaxNode MakeMaybeTailCallLambdaBody(Method method)
        {
            var block = Block(
                MakeSkeletonMethodSerializerLocalDeclaration(method));

            if (method.ResultStruct.Definition.SpecialName == SpecialName.MethodResultStruct)
            {
                block = block.AddStatements(
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName("var"))
                            .AddVariables(
                                VariableDeclarator(_names.ResultLocal.Identifier)
                                .WithInitializer(EqualsValueClause(ObjectCreationExpression(
                                    _names.MakeTypeSyntax(
                                        method.ResultStruct,
                                        method.ResultStruct.Definition,
                                        TypeUsage.DomainClass,
                                        Nullability.NonNullable))
                                    .WithInitializer(
                                        InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                                            .AddExpressions(
                                                MakeSkeletonMethodResultStructInitializer(method).ToArray())))))));
            }

            if (method.Results.Count > 0)
            {
                block = block.AddStatements(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                _names.ResultLocal.IdentifierName,
                                _names.SerializeMethod.IdentifierName))
                            .AddArgumentListArguments(
                                Argument(_names.SerializerLocal.IdentifierName))));
            }

            block = block.AddStatements(
                ReturnStatement(_names.SerializerLocal.IdentifierName));

            return block;
        }

        IEnumerable<StatementSyntax> MakeSkeletonMethodBody(Method method)
        {
            SimpleNameSyntax methodName;

            if (method.GenericParameters.Count == 0)
            {
                methodName = _names.GetCodeIdentifier(method).IdentifierName;
            }
            else
            {
                methodName = GenericName(_names.GetCodeIdentifier(method).Identifier)
                    .AddTypeArgumentListArguments(
                        method.GenericParameters.Select(
                            p => _names.GetGenericTypeParameter(p).IdentifierName)
                        .ToArray());
            }

            var call = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(SkeletonWorder.ImplName),
                    methodName));

            if (method.Params.Count > 0)
            {
                var paramsType = method.ParamsStruct;
                var domainType = _names.MakeTypeSyntax(paramsType, method.ParamsStruct.Definition, TypeUsage.DomainClass, Nullability.NonNullable);

                ExpressionSyntax createDomain = InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(nameof(Capnp.CapnpSerializable)),
                            GenericName(nameof(Capnp.CapnpSerializable.Create))
                                .AddTypeArgumentListArguments(MakeNonNullableType(domainType))))
                            .AddArgumentListArguments(
                                Argument(_names.DeserializerLocal.IdentifierName));

                if (_names.NullableEnable)
                {
                    createDomain = PostfixUnaryExpression(
                        SyntaxKind.SuppressNullableWarningExpression,
                        createDomain);
                }

                if (method.ParamsStruct.Definition.SpecialName == SpecialName.MethodParamsStruct)
                {
                    yield return LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName("var"))
                            .AddVariables(
                                VariableDeclarator(_names.ParamsLocal.Identifier)
                                .WithInitializer(EqualsValueClause(createDomain))));

                    call = call.AddArgumentListArguments(
                        MakeSkeletonMethodCallArgs(method).ToArray());
                }
                else
                {
                    call = call.AddArgumentListArguments(
                        Argument(createDomain));
                }
            }

            call = call.AddArgumentListArguments(
                Argument(
                    _names.CancellationTokenParameter.IdentifierName));

            if (method.Results.Count == 0)
            {
                var awaitCall = AwaitExpression(call);
                yield return ExpressionStatement(awaitCall);
                yield return MakeSkeletonMethodSerializerLocalDeclaration(method);
                yield return ReturnStatement(_names.SerializerLocal.IdentifierName);
            }
            else
            {
                ExpressionSyntax lambdaArg;

                if (method.ResultStruct.Definition.SpecialName == SpecialName.MethodResultStruct)
                {
                    if (method.Results.Count == 1)
                    {
                        lambdaArg = SimpleLambdaExpression(
                            Parameter(Identifier(method.Results.Single().Name)),
                            MakeMaybeTailCallLambdaBody(method));
                    }
                    else
                    {
                        // CodeAnalysis.CSharp 3.2.1 has a bug which prevents us from using AddParameterListParameters. :-(

                        var paramList = new List<SyntaxNodeOrToken>();
                        foreach (var arg in method.Results)
                        {
                            if (paramList.Count > 0)
                                paramList.Add(Token(SyntaxKind.CommaToken));
                            paramList.Add(Parameter(Identifier(arg.Name)));
                        }
                        lambdaArg = ParenthesizedLambdaExpression(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(paramList)),
                            MakeMaybeTailCallLambdaBody(method));
                    }
                }
                else
                {
                    lambdaArg = SimpleLambdaExpression(
                        Parameter(_names.ResultLocal.Identifier),
                        MakeMaybeTailCallLambdaBody(method));

                }

                var maybeTailCall = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(nameof(Capnp.Rpc.Impatient)),
                        IdentifierName(nameof(Capnp.Rpc.Impatient.MaybeTailCall))))
                        .AddArgumentListArguments(
                            Argument(call),
                            Argument(lambdaArg));

                yield return ReturnStatement(maybeTailCall);
            }
        }

        IEnumerable<MemberDeclarationSyntax> MakeSkeletonMethods(TypeDefinition def)
        {
            foreach (var method in def.Methods)
            {
                var methodDecl = MethodDeclaration(
                    _names.Type<Task<Capnp.Rpc.AnswerOrCounterquestion>>(Nullability.NonNullable),
                    _names.GetCodeIdentifier(method).Identifier)
                    .AddParameterListParameters(
                        Parameter(_names.DeserializerLocal.Identifier)
                            .WithType(_names.Type<Capnp.DeserializerState>(Nullability.NonNullable)),
                        Parameter(_names.CancellationTokenParameter.Identifier)
                            .WithType(_names.Type<CancellationToken>(Nullability.NonNullable)))
                    .AddBodyStatements(
                        UsingStatement(
                            Block(
                                MakeSkeletonMethodBody(method).ToArray()))
                            .WithExpression(_names.DeserializerLocal.IdentifierName));

                if (method.Results.Count == 0)
                {
                    methodDecl = methodDecl.AddModifiers(Async);
                }

                if (method.GenericParameters.Count > 0)
                {
                    methodDecl = methodDecl
                        .AddTypeParameterListParameters(MakeTypeParameters(method).ToArray())
                        .AddConstraintClauses(MakeTypeParameterConstraints(method).ToArray());
                }

                yield return methodDecl;
            }
        }

        public MemberDeclarationSyntax MakeSkeleton(TypeDefinition type)
        {
            var name = _names.MakeTypeName(type, NameUsage.Skeleton).Identifier;
            var classDecl = ClassDeclaration(name)
                .AddModifiers(Public)
                .AddBaseListTypes(
                    SimpleBaseType(
                        GenericName(nameof(Capnp.Rpc.Skeleton))
                            .AddTypeArgumentListArguments(
                                _names.MakeGenericTypeName(type, NameUsage.Interface))))
                .AddMembers(
                    // C'tor
                    ConstructorDeclaration(name)
                        .AddModifiers(Public)
                        .AddBodyStatements(
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName(SkeletonWorder.SetMethodTableName))
                                    .AddArgumentListArguments(
                                        MakeSkeletonSetMethodTableArguments(type).ToArray()))),
                    // InterfaceId
                    PropertyDeclaration(_names.Type<ulong>(Nullability.NonNullable), nameof(Capnp.Rpc.Skeleton<object>.InterfaceId))
                        .AddModifiers(Public, Override)
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                ValueOf(type.Id)))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)));

            if (type.GenericParameters.Count > 0)
            {
                classDecl = classDecl
                    .AddTypeParameterListParameters(MakeTypeParameters(type).ToArray())
                    .AddConstraintClauses(MakeTypeParameterConstraints(type).ToArray());
            }

            classDecl = classDecl.AddMembers(MakeSkeletonMethods(type).ToArray());

            return classDecl;
        }

        public bool RequiresPipeliningSupport(TypeDefinition type)
        {
            return type.Methods.Any(m => ExpandPipeliningPaths(m).Any());
        }

        IEnumerable<IReadOnlyList<Field>> ExpandPipeliningPaths(Method method)
        {
            var stack = new Stack<List<Field>>();
            foreach (var field in method.ResultStruct.Fields)
            {
                stack.Push(new List<Field>() { field });
            }

            while (stack.Count > 0)
            {
                var path = stack.Pop();
                var last = path[path.Count - 1];

                switch (last.Type.Tag)
                {
                    case TypeTag.Interface:
                    case TypeTag.CapabilityPointer:
                        yield return path;
                        break;

                    case TypeTag.Struct:
                        foreach (var field in last.Type.Fields)
                        {
                            if (path.Contains(field))
                            {
                                // Recursive structs protection
                                continue;
                            }

                            var copy = new List<Field>();
                            copy.AddRange(path);
                            copy.Add(field);
                            stack.Push(copy);
                        }
                        break;
                }
            }
        }

        readonly HashSet<(string, string)> _existingExtensionMethods = new HashSet<(string, string)>();

        LocalFunctionStatementSyntax MakeLocalAwaitProxyFunction(Method method, IReadOnlyList<Field> path)
        {
            var members = new List<Name>();
            IEnumerable<Field> fields = path;

            if (method.Results.Count >= 2)
            {
                int index = Array.IndexOf(method.ResultStruct.Fields.ToArray(), path[0]) + 1;
                members.Add(new Name($"Item{index}"));
                fields = path.Skip(1);
            }

            foreach (var field in fields)
            {
                members.Add(_names.GetCodeIdentifier(field));
            }

            ExpressionSyntax memberAccess = 
                ParenthesizedExpression(
                    AwaitExpression(
                        _names.TaskParameter.IdentifierName));

            memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                memberAccess,
                members.First().IdentifierName);

            foreach (var member in members.Skip(1))
            {
                memberAccess = ConditionalAccessExpression(
                    memberAccess,
                    MemberBindingExpression(member.IdentifierName));
            }

            var idisposable = _names.MakeNullableRefType(IdentifierName(nameof(IDisposable)));

            return LocalFunctionStatement(
                    GenericName(
                        Identifier(nameof(Task)))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                idisposable))),
                    _names.AwaitProxy.Identifier)
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.AsyncKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(memberAccess))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }

        public IEnumerable<MemberDeclarationSyntax> MakePipeliningSupport(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                foreach (var path in ExpandPipeliningPaths(method))
                {
                    if (path.Count == 1 && path[0].Offset == 0)
                    {
                        // The "trivial path" is already covered by the "Eager" extension method.
                        continue;
                    }

                    var accessPath = _names.MakeMemberAccessPathFieldName(method, path);
                    var methodName = _names.MakePipeliningSupportExtensionMethodName(path);
                    var capType = path[path.Count - 1].Type;
                    var capTypeSyntax = _names.MakeTypeSyntax(capType, null, TypeUsage.DomainClass, Nullability.NonNullable);

                    if (!_existingExtensionMethods.Add((capTypeSyntax.ToString(), methodName.ToString())))
                    {
                        continue;
                    }

                    var pathDecl = FieldDeclaration(
                        VariableDeclaration(
                            IdentifierName(nameof(Capnp.Rpc.MemberAccessPath)))
                        .AddVariables(
                            VariableDeclarator(
                                accessPath.Identifier)
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                        IdentifierName(nameof(Capnp.Rpc.MemberAccessPath)))
                                        .AddArgumentListArguments(
                                            path.Select(
                                                f => Argument(
                                                        LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                                        Literal(f.Offset)))).ToArray())))))
                        .AddModifiers(Static, Readonly);


                    var methodDecl = MethodDeclaration(capTypeSyntax, methodName.Identifier)
                        .AddModifiers(Public, Static)
                        .AddParameterListParameters(
                            Parameter(
                                _names.TaskParameter.Identifier)
                            .AddModifiers(This)
                            .WithType(TransformReturnType(method)))
                        .AddBodyStatements(
                            MakeLocalAwaitProxyFunction(method, path),
                            ReturnStatement(
                                CastExpression(
                                    capTypeSyntax,
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(nameof(Capnp.Rpc.CapabilityReflection)),
                                            GenericName(
                                                Identifier(nameof(Capnp.Rpc.CapabilityReflection.CreateProxy)))
                                            .AddTypeArgumentListArguments(
                                                capTypeSyntax)))
                                    .AddArgumentListArguments(
                                        Argument(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(nameof(Capnp.Rpc.Impatient)),
                                                    IdentifierName(nameof(Capnp.Rpc.Impatient.Access))))
                                            .AddArgumentListArguments(
                                                Argument(
                                                    _names.TaskParameter.IdentifierName),
                                                Argument(
                                                    accessPath.IdentifierName),
                                                Argument(
                                                    InvocationExpression(
                                                        _names.AwaitProxy.IdentifierName))))))));

                    yield return pathDecl;
                    yield return methodDecl;
                }
            }
        }
    }
}
