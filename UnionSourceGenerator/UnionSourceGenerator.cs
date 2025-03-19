//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Linq;
using Microsoft.CodeAnalysis;

namespace UnionSourceGenerator;

/// <summary>
///     A sample source generator that creates C# classes based on the text file (in this case, Domain Driven Design
///     ubiquitous language registry).
///     When using a simple text file as a baseline, we can create a non-incremental source generator.
/// </summary>
[Generator]
public class UnionSourceGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
			return;
		foreach (var partialStruct in syntaxReceiver.PartialStructs)
		{
			var semanticModel = context.Compilation.GetSemanticModel(partialStruct.SyntaxTree);
			var structSymbol = semanticModel.GetDeclaredSymbol(partialStruct) as INamedTypeSymbol;

			if (structSymbol == null)
				continue;

			var iUnionInterface = structSymbol.AllInterfaces
				.FirstOrDefault(i => i.Name == "IUnion" && i.TypeArguments.Length > 0);

			if (iUnionInterface != null)
			{
				var typeArguments = iUnionInterface.TypeArguments;
				if (typeArguments.Length == 2)
				{
					var firstTypeFullName = typeArguments[0].ToDisplayString();
					var secondTypeFullName = typeArguments[1].ToDisplayString();
					var @namespace = structSymbol.ContainingNamespace.ToDisplayString();
					var structName = structSymbol.Name;

					var generatedCode = GenerateUnionTwo(@namespace, structName, firstTypeFullName, secondTypeFullName);
					context.AddSource($"{structName}.g.cs", generatedCode);
				}
				else if (typeArguments.Length == 3)
				{
					var firstTypeFullName = typeArguments[0].ToDisplayString();
					var secondTypeFullName = typeArguments[1].ToDisplayString();
					var thirdTypeFullName = typeArguments[2].ToDisplayString();
					var @namespace = structSymbol.ContainingNamespace.ToDisplayString();
					var structName = structSymbol.Name;

					var generatedCode = GenerateUnionThree(@namespace, structName, firstTypeFullName,
						secondTypeFullName, thirdTypeFullName);
					context.AddSource($"{structName}.g.cs", generatedCode);
				}
				else if (typeArguments.Length == 4)
				{
					var firstTypeFullName = typeArguments[0].ToDisplayString();
					var secondTypeFullName = typeArguments[1].ToDisplayString();
					var thirdTypeFullName = typeArguments[2].ToDisplayString();
					var forthTypeFullName = typeArguments[3].ToDisplayString();
					var @namespace = structSymbol.ContainingNamespace.ToDisplayString();
					var structName = structSymbol.Name;

					var generatedCode = GenerateUnionFour(@namespace, structName, firstTypeFullName, secondTypeFullName,
						thirdTypeFullName, forthTypeFullName);
					context.AddSource($"{structName}.g.cs", generatedCode);
				}
			}
		}
	}

	private static string GenerateUnionTwo(string @namespace, string structName, string firstTypeFullName,
		string secondTypeFullName)
	{
		return $@"
#nullable enable
using System.Runtime.InteropServices;

namespace {@namespace};
[StructLayout(LayoutKind.Explicit)]
public readonly partial struct {structName}
{{
	private const byte IsFirstType = 0;
	private const byte IsSecondType = 1;
	[FieldOffset(0)]
	private readonly byte _elementSet;
	[FieldOffset(8)]
	private readonly {firstTypeFullName} _first;
	[FieldOffset(8)]
	private readonly {secondTypeFullName} _second;

    /// <summary>
    ///    Type of stored element.
    /// </summary>
	public Type ElementType
	{{
		get
		{{
			switch (_elementSet)
			{{
				case IsFirstType:
					return typeof({firstTypeFullName});
				case IsSecondType:
					return typeof({secondTypeFullName});;
				default:
					throw new NotImplementedException(""Unreachable code"");
			}}
		}}
	}}
	
    /// <summary>
    ///    Creates a new instance of the union from {firstTypeFullName}.
    /// </summary>
	public {structName}({firstTypeFullName} element)
	{{
		_elementSet = IsFirstType;
		_second = default!;
		_first = element;
	}}
	
    /// <summary>
    ///    Creates a new instance of the union from {secondTypeFullName}.
    /// </summary>
	public {structName}({secondTypeFullName} element)
	{{
		_elementSet = IsSecondType;
		_first = default!;
		_second = element;
	}}
	
    /// <summary>
    ///    Matches the value of the union and returns the result of the matching function.
    /// </summary>
	public bool Is<T>()
	{{
		if (typeof(T) == typeof({firstTypeFullName}))
			return _elementSet == IsFirstType;
		else if (typeof(T) == typeof({secondTypeFullName}))
			return _elementSet == IsSecondType;
		else
			return false;
	}}

	/// <summary>
    ///    Matches the value of the union and invokes matching function.
    /// </summary>
	public void Match(Action<{firstTypeFullName}>? first = null, Action<{secondTypeFullName}>? second = null)
	{{
		switch (_elementSet)
		{{
			case IsFirstType:
				first?.Invoke(_first);
				break;
			case IsSecondType:
				second?.Invoke(_second);
				break;
			default:
				throw new InvalidCastException(""Union was not initialized and doesn't hold any value."");
		}}
	}}
	
	/// <summary>
    ///    Matches the value of the union and returns the result of the matching function.
    /// </summary>
	public T Match<T>(Func<{firstTypeFullName}, T> first, Func<{secondTypeFullName}, T> second)
	{{
		switch (_elementSet)
		{{
			case IsFirstType:
				return first(_first);
			case IsSecondType:
				return second(_second);
			default:
				throw new InvalidCastException(""Union was not initialized and doesn't hold any value."");
		}}
	}}
	
    /// <summary>
    ///   Converts the union to a value of type {firstTypeFullName}.
    /// </summary>
	public static explicit operator {firstTypeFullName}({structName} union)
	{{
		if (union._elementSet == IsFirstType)
			return union._first;
		throw new InvalidCastException(
			$""Union doesn't hold a value of type {firstTypeFullName} but of type {{union.ElementType}} instead."");
	}}

    /// <summary>
    ///   Converts the union to a value of type {secondTypeFullName}.
    /// </summary>
	public static explicit operator {secondTypeFullName}({structName} union)
	{{
		if (union._elementSet == IsFirstType)
			return union._second;
		throw new InvalidCastException(
			$""Union doesn't hold a value of type {secondTypeFullName} but of type {{union.ElementType}} instead."");
	}}
}}
";
	}

	private static string GenerateUnionThree(string @namespace, string structName, string firstTypeFullName,
		string secondTypeFullName, string thirdTypeFullName)
	{
		return $@"
#nullable enable
using System.Runtime.InteropServices;

namespace {@namespace};
[StructLayout(LayoutKind.Explicit)]
public readonly partial struct {structName}
{{
    private const byte IsFirstType = 0;
    private const byte IsSecondType = 1;
    private const byte IsThirdType = 2;
	[FieldOffset(0)]
    private readonly byte _elementSet;
    [FieldOffset(8)]
    private readonly {firstTypeFullName} _first;
	[FieldOffset(8)]
    private readonly {secondTypeFullName} _second;
	[FieldOffset(8)]
    private readonly {thirdTypeFullName} _third;
	
    /// <summary>
    ///    Type of stored element.
    /// </summary>
    public Type ElementType
    {{
        get
        {{
            return _elementSet switch
            {{
                IsFirstType => typeof({firstTypeFullName}),
                IsSecondType => typeof({secondTypeFullName}),
                IsThirdType => typeof({thirdTypeFullName}),
                _ => throw new NotImplementedException(""Unreachable code"")
            }};
        }}
    }}
	
    /// <summary>
    ///    Creates a new instance of the union from {firstTypeFullName}.
    /// </summary>
    public {structName}({firstTypeFullName} element)
    {{
        _elementSet = IsFirstType;
        _second = default!;
        _third = default!;
        _first = element;
    }}
	
	
    /// <summary>
    ///    Creates a new instance of the union from {secondTypeFullName}.
    /// </summary>
    public {structName}({secondTypeFullName} element)
    {{
        _elementSet = IsSecondType;
        _first = default!;
        _third = default!;
        _second = element;
    }}

	
    /// <summary>
    ///    Creates a new instance of the union from {thirdTypeFullName}.
    /// </summary>
    public {structName}({thirdTypeFullName} element)
    {{
        _elementSet = IsThirdType;
        _first = default!;
        _second = default!;
        _third = element;
    }}
	
    /// <summary>
    ///    Matches the value of the union and returns the result of the matching function.
    /// </summary>
    public bool Is<T>()
    {{
        if (typeof(T) == typeof({firstTypeFullName}))
            return _elementSet == IsFirstType;
        else if (typeof(T) == typeof({secondTypeFullName}))
            return _elementSet == IsSecondType;
        else if (typeof(T) == typeof({thirdTypeFullName}))
            return _elementSet == IsThirdType;
        else
            return false;
    }}

	/// <summary>
    ///    Matches the value of the union and invokes matching function.
    /// </summary>
    public void Match(Action<{firstTypeFullName}>? first = null, Action<{secondTypeFullName}>? second = null, Action<{thirdTypeFullName}>? third = null)
    {{
        switch (_elementSet)
        {{
            case IsFirstType:
                first?.Invoke(_first);
                break;
            case IsSecondType:
                second?.Invoke(_second);
                break;
            case IsThirdType:
                third?.Invoke(_third);
                break;
            default:
                throw new InvalidCastException(""Union was not initialized and doesn't hold any value."");
        }}
    }}

	/// <summary>
    ///    Matches the value of the union and returns the result of the matching function.
    /// </summary>
    public T Match<T>(Func<{firstTypeFullName}, T> first, Func<{secondTypeFullName}, T> second, Func<{thirdTypeFullName}, T> third)
    {{
        return _elementSet switch
        {{
            IsFirstType => first(_first),
            IsSecondType => second(_second),
            IsThirdType => third(_third),
            _ => throw new InvalidCastException(""Union was not initialized and doesn't hold any value."")
        }};
    }}
	
    /// <summary>
    ///   Converts the union to a value of type {firstTypeFullName}.
    /// </summary>
    public static explicit operator {firstTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsFirstType)
            return union._first;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {firstTypeFullName} but of type {{union.ElementType}} instead."");
    }}

    /// <summary>
    ///   Converts the union to a value of type {secondTypeFullName}.
    /// </summary>
    public static explicit operator {secondTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsSecondType)
            return union._second;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {secondTypeFullName} but of type {{union.ElementType}} instead."");
    }}
	
    /// <summary>
    ///   Converts the union to a value of type {thirdTypeFullName}.
    /// </summary>
    public static explicit operator {thirdTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsThirdType)
            return union._third;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {thirdTypeFullName} but of type {{union.ElementType}} instead."");
    }}
}}
";
	}

	private static string GenerateUnionFour(string @namespace, string structName, string firstTypeFullName,
		string secondTypeFullName, string thirdTypeFullName, string fourthTypeFullName)
	{
		return $@"
#nullable enable
using System.Runtime.InteropServices;

namespace {@namespace};
[StructLayout(LayoutKind.Explicit)]
public readonly partial struct {structName}
{{
    private const byte IsFirstType = 0;
    private const byte IsSecondType = 1;
    private const byte IsThirdType = 2;
    private const byte IsFourthType = 3;
	[FieldOffset(0)]
    private readonly byte _elementSet;
	[FieldOffset(8)]
    private readonly {firstTypeFullName} _first;
    [FieldOffset(8)]
    private readonly {secondTypeFullName} _second;
    [FieldOffset(8)]
    private readonly {thirdTypeFullName} _third;
    [FieldOffset(8)]
    private readonly {fourthTypeFullName} _fourth;
	
    /// <summary>
    ///    Type of stored element.
    /// </summary>
    public Type ElementType
    {{
        get
        {{
            return _elementSet switch
            {{
                IsFirstType => typeof({firstTypeFullName}),
                IsSecondType => typeof({secondTypeFullName}),
                IsThirdType => typeof({thirdTypeFullName}),
                IsFourthType => typeof({fourthTypeFullName}),
                _ => throw new NotImplementedException(""Unreachable code"")
            }};
        }}
    }}

    /// <summary>
    ///    Creates a new instance of the union from {{firstTypeFullName}}.
    /// </summary>
    public {structName}({firstTypeFullName} element)
    {{
        _elementSet = IsFirstType;
        _second = default!;
        _third = default!;
        _fourth = default!;
        _first = element;
    }}
	
    /// <summary>
    ///    Creates a new instance of the union from {secondTypeFullName}.
    /// </summary>
    public {structName}({secondTypeFullName} element)
    {{
        _elementSet = IsSecondType;
        _first = default!;
        _third = default!;
        _fourth = default!;
        _second = element;
    }}
	
    /// <summary>
    ///    Creates a new instance of the union from {thirdTypeFullName}.
    /// </summary>
    public {structName}({thirdTypeFullName} element)
    {{
        _elementSet = IsThirdType;
        _first = default!;
        _second = default!;
        _fourth = default!;
        _third = element;
    }}
	
    /// <summary>
    ///    Creates a new instance of the union from {fourthTypeFullName}.
    /// </summary>
    public {structName}({fourthTypeFullName} element)
    {{
        _elementSet = IsFourthType;
        _first = default!;
        _second = default!;
        _third = default!;
        _fourth = element;
    }}

    /// <summary>
    ///    Returns true if the union holds a value of type T.
    /// </summary>
    public bool Is<T>()
    {{
        if (typeof(T) == typeof({firstTypeFullName}))
            return _elementSet == IsFirstType;
        else if (typeof(T) == typeof({secondTypeFullName}))
            return _elementSet == IsSecondType;
        else if (typeof(T) == typeof({thirdTypeFullName}))
            return _elementSet == IsThirdType;
        else if (typeof(T) == typeof({fourthTypeFullName}))
            return _elementSet == IsFourthType;
        else
            return false;
    }}
	
	/// <summary>
    ///    Matches the value of the union and invokes matching function.
    /// </summary>
    public void Match(Action<{firstTypeFullName}>? first = null, Action<{secondTypeFullName}>? second = null, Action<{thirdTypeFullName}>? third = null, Action<{fourthTypeFullName}>? fourth = null)
    {{
        switch (_elementSet)
        {{
            case IsFirstType:
                first?.Invoke(_first);
                break;
            case IsSecondType:
                second?.Invoke(_second);
                break;
            case IsThirdType:
                third?.Invoke(_third);
                break;
            case IsFourthType:
                fourth?.Invoke(_fourth);
                break;
            default:
                throw new InvalidCastException(""Union was not initialized and doesn't hold any value."");
        }}
    }}

	/// <summary>
    ///    Matches the value of the union and returns the result of the matching function.
    /// </summary>
    public T Match<T>(Func<{firstTypeFullName}, T> first, Func<{secondTypeFullName}, T> second, Func<{thirdTypeFullName}, T> third, Func<{fourthTypeFullName}, T> fourth)
    {{
        return _elementSet switch
        {{
            IsFirstType => first(_first),
            IsSecondType => second(_second),
            IsThirdType => third(_third),
            IsFourthType => fourth(_fourth),
            _ => throw new InvalidCastException(""Union was not initialized and doesn't hold any value."")
        }};
    }}
	
    /// <summary>
    ///   Converts the union to a value of type {firstTypeFullName}.
    /// </summary>
    public static explicit operator {firstTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsFirstType)
            return union._first;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {firstTypeFullName} but of type {{union.ElementType}} instead."");
    }}

    /// <summary>
    ///   Converts the union to a value of type {secondTypeFullName}.
    /// </summary>
    public static explicit operator {secondTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsSecondType)
            return union._second;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {secondTypeFullName} but of type {{union.ElementType}} instead."");
    }}

    /// <summary>
    ///   Converts the union to a value of type {thirdTypeFullName}.
    /// </summary>
    public static explicit operator {thirdTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsThirdType)
            return union._third;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {thirdTypeFullName} but of type {{union.ElementType}} instead."");
    }}

    /// <summary>
    ///   Converts the union to a value of type {fourthTypeFullName}.
    /// </summary>
    public static explicit operator {fourthTypeFullName}({structName} union)
    {{
        if (union._elementSet == IsFourthType)
            return union._fourth;
        throw new InvalidCastException(
            $""Union doesn't hold a value of type {fourthTypeFullName} but of type {{union.ElementType}} instead."");
    }}
}}
";
	}
}