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

Scenario: Combining frontend and backend
	Given capnp.exe is installed on my system
	And I have a schema "UnitTest1.capnp"
	When I try to generate code from that schema
	Then code generation must succeed

Scenario: Missing frontend
	Given capnp.exe is not installed on my system
	And I have a schema "UnitTest1.capnp"
	When I try to generate code from that schema
	Then the invocation must fail

Scenario: Schema without ID
	Given capnp.exe is installed on my system
	And I have a schema "Empty1.capnp"
	When I try to generate code from that schema
	Then the invocation must fail
	And the reason must be bad input
	And the error output must contain "File does not declare an ID"

Scenario: Multiple errors
	Given capnp.exe is installed on my system
	And I have a schema "invalid.capnp"
	When I try to generate code from that schema
	Then the invocation must fail
	And the reason must be bad input
	And the error output must contain multiple messages

Scenario Outline: Valid generator output
	Given I have a binary code generator request <bin>
	And I enable generation of nullable reference types according to <nullablegen>
	And I enable the compiler support of nullable reference types according to <nullablesupp>
	When I invoke capnpc-csharp
	Then the invocation must succeed and attempting to compile the generated code gives <outcome>

Examples:
    | bin               | nullablegen | nullablesupp | outcome  |
    | test.capnp.bin    | false       | false        | success  |
    | test.capnp.bin    | true        | false        | success   |
    | test.capnp.bin    | false       | true         | warnings |
    | test.capnp.bin    | true        | true         | success  |
    | Issue19.capnp.bin | false       | false        | success  |
    | Issue21.capnp.bin | false       | false        | success  |
    | Issue22.capnp.bin | false       | false        | success  |
    | NullableDisable.capnp.bin   | true        | false        | success  |
    | NullableDisable.capnp.bin   | true        | true         | warnings |
    | NullableEnable.capnp.bin    | false       | true         | success  |
	| NullableEnable.capnp.bin    | false       | false        | errors   |