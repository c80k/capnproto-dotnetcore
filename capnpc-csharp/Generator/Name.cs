using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapnpC.Generator
{
    class Name
    {
        readonly string _name;

        public Name(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            IdentifierName = SyntaxFactory.IdentifierName(_name);
            Identifier = SyntaxFactory.Identifier(_name);
            VariableDeclarator = SyntaxFactory.VariableDeclarator(_name);
        }

        public IdentifierNameSyntax IdentifierName { get; } 
        public SyntaxToken Identifier { get; }
        public VariableDeclaratorSyntax VariableDeclarator { get; }

        public override string ToString() => _name;

        public override bool Equals(object obj)
        {
            return obj is Name other && _name == other._name;
        }

        public override int GetHashCode() => _name.GetHashCode();
    }
}
