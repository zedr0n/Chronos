//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/mbp/Dev/Chronos/Chronos.Console/Grammar/Chronos.g4 by ANTLR 4.7

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IChronosListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class ChronosBaseListener : IChronosListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.command"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCommand([NotNull] ChronosParser.CommandContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.command"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCommand([NotNull] ChronosParser.CommandContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.query"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQuery([NotNull] ChronosParser.QueryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.query"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQuery([NotNull] ChronosParser.QueryContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.start"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStart([NotNull] ChronosParser.StartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.start"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStart([NotNull] ChronosParser.StartContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.stop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStop([NotNull] ChronosParser.StopContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.stop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStop([NotNull] ChronosParser.StopContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.replay"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReplay([NotNull] ChronosParser.ReplayContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.replay"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReplay([NotNull] ChronosParser.ReplayContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.create"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreate([NotNull] ChronosParser.CreateContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.create"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreate([NotNull] ChronosParser.CreateContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.createCoin"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateCoin([NotNull] ChronosParser.CreateCoinContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.createCoin"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateCoin([NotNull] ChronosParser.CreateCoinContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.createBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateBag([NotNull] ChronosParser.CreateBagContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.createBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateBag([NotNull] ChronosParser.CreateBagContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.trackAsset"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTrackAsset([NotNull] ChronosParser.TrackAssetContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.trackAsset"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTrackAsset([NotNull] ChronosParser.TrackAssetContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.add"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAdd([NotNull] ChronosParser.AddContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.add"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAdd([NotNull] ChronosParser.AddContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.assetToBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssetToBag([NotNull] ChronosParser.AssetToBagContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.assetToBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssetToBag([NotNull] ChronosParser.AssetToBagContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.remove"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRemove([NotNull] ChronosParser.RemoveContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.remove"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRemove([NotNull] ChronosParser.RemoveContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.removeAssetFromBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRemoveAssetFromBag([NotNull] ChronosParser.RemoveAssetFromBagContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.removeAssetFromBag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRemoveAssetFromBag([NotNull] ChronosParser.RemoveAssetFromBagContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.to"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTo([NotNull] ChronosParser.ToContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.to"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTo([NotNull] ChronosParser.ToContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.bag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBag([NotNull] ChronosParser.BagContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.bag"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBag([NotNull] ChronosParser.BagContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.bags"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBags([NotNull] ChronosParser.BagsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.bags"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBags([NotNull] ChronosParser.BagsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.date"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDate([NotNull] ChronosParser.DateContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.date"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDate([NotNull] ChronosParser.DateContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.asset"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAsset([NotNull] ChronosParser.AssetContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.asset"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAsset([NotNull] ChronosParser.AssetContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.duration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDuration([NotNull] ChronosParser.DurationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.duration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDuration([NotNull] ChronosParser.DurationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.name"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterName([NotNull] ChronosParser.NameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.name"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitName([NotNull] ChronosParser.NameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.ticker"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTicker([NotNull] ChronosParser.TickerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.ticker"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTicker([NotNull] ChronosParser.TickerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.quantity"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQuantity([NotNull] ChronosParser.QuantityContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.quantity"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQuantity([NotNull] ChronosParser.QuantityContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.guid"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGuid([NotNull] ChronosParser.GuidContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.guid"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGuid([NotNull] ChronosParser.GuidContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.guidOptional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGuidOptional([NotNull] ChronosParser.GuidOptionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.guidOptional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGuidOptional([NotNull] ChronosParser.GuidOptionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.newGuid"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNewGuid([NotNull] ChronosParser.NewGuidContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.newGuid"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNewGuid([NotNull] ChronosParser.NewGuidContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.bagDescriptor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBagDescriptor([NotNull] ChronosParser.BagDescriptorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.bagDescriptor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBagDescriptor([NotNull] ChronosParser.BagDescriptorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ChronosParser.assetDescriptor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssetDescriptor([NotNull] ChronosParser.AssetDescriptorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ChronosParser.assetDescriptor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssetDescriptor([NotNull] ChronosParser.AssetDescriptorContext context) { }

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
