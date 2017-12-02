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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="ChronosParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public interface IChronosVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.command"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCommand([NotNull] ChronosParser.CommandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.query"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuery([NotNull] ChronosParser.QueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.create"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCreate([NotNull] ChronosParser.CreateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.createCoin"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCreateCoin([NotNull] ChronosParser.CreateCoinContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.createBag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCreateBag([NotNull] ChronosParser.CreateBagContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.trackAsset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTrackAsset([NotNull] ChronosParser.TrackAssetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.add"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAdd([NotNull] ChronosParser.AddContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.addAssetToBag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddAssetToBag([NotNull] ChronosParser.AddAssetToBagContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.bags"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBags([NotNull] ChronosParser.BagsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.asset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAsset([NotNull] ChronosParser.AssetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.duration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDuration([NotNull] ChronosParser.DurationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitName([NotNull] ChronosParser.NameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.ticker"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTicker([NotNull] ChronosParser.TickerContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.quantity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuantity([NotNull] ChronosParser.QuantityContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.guid"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGuid([NotNull] ChronosParser.GuidContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.guidOptional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGuidOptional([NotNull] ChronosParser.GuidOptionalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.newGuid"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNewGuid([NotNull] ChronosParser.NewGuidContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.bagId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBagId([NotNull] ChronosParser.BagIdContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.assetId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssetId([NotNull] ChronosParser.AssetIdContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.bagDescriptor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBagDescriptor([NotNull] ChronosParser.BagDescriptorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ChronosParser.assetDescriptor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssetDescriptor([NotNull] ChronosParser.AssetDescriptorContext context);
}
