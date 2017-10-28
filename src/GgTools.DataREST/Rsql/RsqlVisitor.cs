//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Rsql.g by ANTLR 4.6

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace GgTools.DataREST.Rsql {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="RsqlParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6")]
public interface IRsqlVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.eval"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEval([NotNull] RsqlParser.EvalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.or"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOr([NotNull] RsqlParser.OrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.and"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnd([NotNull] RsqlParser.AndContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.constraint"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstraint([NotNull] RsqlParser.ConstraintContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGroup([NotNull] RsqlParser.GroupContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.comparison"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparison([NotNull] RsqlParser.ComparisonContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.comparator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparator([NotNull] RsqlParser.ComparatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.comp_fiql"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComp_fiql([NotNull] RsqlParser.Comp_fiqlContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.comp_alt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComp_alt([NotNull] RsqlParser.Comp_altContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.reserved"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReserved([NotNull] RsqlParser.ReservedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.unreserved"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnreserved([NotNull] RsqlParser.UnreservedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.escaped"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEscaped([NotNull] RsqlParser.EscapedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.single_quote"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSingle_quote([NotNull] RsqlParser.Single_quoteContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArguments([NotNull] RsqlParser.ArgumentsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValue([NotNull] RsqlParser.ValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RsqlParser.selector"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSelector([NotNull] RsqlParser.SelectorContext context);
}
} // namespace GgTools.DataREST.Rsql