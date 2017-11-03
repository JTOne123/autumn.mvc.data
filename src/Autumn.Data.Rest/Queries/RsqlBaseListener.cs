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

namespace Autumn.Data.Rest.Queries {

using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IRsqlListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6")]
[System.CLSCompliant(false)]
public partial class RsqlBaseListener : IRsqlListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.selector"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSelector([NotNull] RsqlParser.SelectorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.selector"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSelector([NotNull] RsqlParser.SelectorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.eval"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEval([NotNull] RsqlParser.EvalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.eval"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEval([NotNull] RsqlParser.EvalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.or"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOr([NotNull] RsqlParser.OrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.or"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOr([NotNull] RsqlParser.OrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.and"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnd([NotNull] RsqlParser.AndContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.and"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnd([NotNull] RsqlParser.AndContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.constraint"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstraint([NotNull] RsqlParser.ConstraintContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.constraint"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstraint([NotNull] RsqlParser.ConstraintContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGroup([NotNull] RsqlParser.GroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGroup([NotNull] RsqlParser.GroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.comparison"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComparison([NotNull] RsqlParser.ComparisonContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.comparison"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComparison([NotNull] RsqlParser.ComparisonContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.comparator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComparator([NotNull] RsqlParser.ComparatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.comparator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComparator([NotNull] RsqlParser.ComparatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.comp_fiql"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComp_fiql([NotNull] RsqlParser.Comp_fiqlContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.comp_fiql"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComp_fiql([NotNull] RsqlParser.Comp_fiqlContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.comp_alt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComp_alt([NotNull] RsqlParser.Comp_altContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.comp_alt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComp_alt([NotNull] RsqlParser.Comp_altContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.reserved"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReserved([NotNull] RsqlParser.ReservedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.reserved"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReserved([NotNull] RsqlParser.ReservedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.single_quote"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSingle_quote([NotNull] RsqlParser.Single_quoteContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.single_quote"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSingle_quote([NotNull] RsqlParser.Single_quoteContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArguments([NotNull] RsqlParser.ArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArguments([NotNull] RsqlParser.ArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="RsqlParser.value"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValue([NotNull] RsqlParser.ValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="RsqlParser.value"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValue([NotNull] RsqlParser.ValueContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace Autumn.Data.Rest.Queries
