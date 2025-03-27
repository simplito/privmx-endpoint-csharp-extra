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

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnionSourceGenerator;

public class SyntaxReceiver : ISyntaxReceiver
{
	public List<StructDeclarationSyntax> PartialStructs { get; } = new();

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		// Filter for partial struct declarations that implement IUnion interface
		if (syntaxNode is StructDeclarationSyntax structDeclaration &&
		    structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) &&
		    structDeclaration.BaseList?.Types.Any(t => t.ToString().StartsWith("IUnion")) == true)
			PartialStructs.Add(structDeclaration);
	}
}