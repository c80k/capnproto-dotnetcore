Feature: CodeGenerator
	In order to ensure that the generator backend produces valid output
	As a contributor
	I want to get notified when there is any deviation from reference output

Scenario: Comparing backend output with reference
	Given I have a binary code generator request "test.capnp.bin"
	And my reference output is "test.cs"
	When I invoke capnpc-csharp
	Then the generated output must match the reference

Scenario Outline: Invalid binary code generator requests
	Given I have a binary code generator request <bin>
	When I invoke capnpc-csharp
	Then the invocation must fail

Examples:
    | bin        |
    | null.bin   |
    | test.cs    |