// Module name: UnionSourceGenerator
// File name: SyntaxReceiver.cs
// Last edit: 2025-02-17 08:48 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

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