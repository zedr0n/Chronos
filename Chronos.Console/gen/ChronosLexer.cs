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

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class ChronosLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		CREATE=1, COIN=2, TRACK=3, GUID=4, BAG=5, ADD=6, BAGS=7, STOP=8, TO=9, 
		WORD=10, WHITESPACE=11, NEWLINE=12, NUMBER=13, DOUBLE=14;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"C", "R", "E", "A", "B", "T", "O", "I", "N", "K", "W", "G", "U", "D", 
		"S", "P", "LOWERCASE", "UPPERCASE", "DIGIT", "CREATE", "COIN", "TRACK", 
		"GUID", "BAG", "ADD", "BAGS", "STOP", "TO", "WORD", "WHITESPACE", "NEWLINE", 
		"NUMBER", "DOUBLE"
	};


	public ChronosLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public ChronosLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
	};
	private static readonly string[] _SymbolicNames = {
		null, "CREATE", "COIN", "TRACK", "GUID", "BAG", "ADD", "BAGS", "STOP", 
		"TO", "WORD", "WHITESPACE", "NEWLINE", "NUMBER", "DOUBLE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Chronos.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static ChronosLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x10', '\xC1', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', '\x18', '\t', 
		'\x18', '\x4', '\x19', '\t', '\x19', '\x4', '\x1A', '\t', '\x1A', '\x4', 
		'\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', '\x1C', '\x4', '\x1D', '\t', 
		'\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', '\x1F', '\t', '\x1F', '\x4', 
		' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', '\"', '\t', '\"', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\a', '\x3', '\a', '\x3', '\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', 
		'\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', 
		'\f', '\x3', '\r', '\x3', '\r', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', 
		'\x3', '\xF', '\x3', '\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', 
		'\x3', '\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', '\x3', '\x14', 
		'\x3', '\x14', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', 
		'\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x16', '\x3', '\x16', 
		'\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', '\x17', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x18', 
		'\x3', '\x18', '\x3', '\x18', '\x3', '\x18', '\x3', '\x18', '\x3', '\x19', 
		'\x3', '\x19', '\x3', '\x19', '\x3', '\x19', '\x3', '\x1A', '\x3', '\x1A', 
		'\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1B', '\x3', '\x1B', '\x3', '\x1B', 
		'\x3', '\x1B', '\x3', '\x1B', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', 
		'\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1D', 
		'\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1E', '\x6', '\x1E', '\x9B', '\n', 
		'\x1E', '\r', '\x1E', '\xE', '\x1E', '\x9C', '\x3', '\x1F', '\x6', '\x1F', 
		'\xA0', '\n', '\x1F', '\r', '\x1F', '\xE', '\x1F', '\xA1', '\x3', '\x1F', 
		'\x3', '\x1F', '\x3', ' ', '\x5', ' ', '\xA7', '\n', ' ', '\x3', ' ', 
		'\x3', ' ', '\x6', ' ', '\xAB', '\n', ' ', '\r', ' ', '\xE', ' ', '\xAC', 
		'\x3', '!', '\x6', '!', '\xB0', '\n', '!', '\r', '!', '\xE', '!', '\xB1', 
		'\x3', '\"', '\x6', '\"', '\xB5', '\n', '\"', '\r', '\"', '\xE', '\"', 
		'\xB6', '\x3', '\"', '\x5', '\"', '\xBA', '\n', '\"', '\x3', '\"', '\a', 
		'\"', '\xBD', '\n', '\"', '\f', '\"', '\xE', '\"', '\xC0', '\v', '\"', 
		'\x2', '\x2', '#', '\x3', '\x2', '\x5', '\x2', '\a', '\x2', '\t', '\x2', 
		'\v', '\x2', '\r', '\x2', '\xF', '\x2', '\x11', '\x2', '\x13', '\x2', 
		'\x15', '\x2', '\x17', '\x2', '\x19', '\x2', '\x1B', '\x2', '\x1D', '\x2', 
		'\x1F', '\x2', '!', '\x2', '#', '\x2', '%', '\x2', '\'', '\x2', ')', '\x3', 
		'+', '\x4', '-', '\x5', '/', '\x6', '\x31', '\a', '\x33', '\b', '\x35', 
		'\t', '\x37', '\n', '\x39', '\v', ';', '\f', '=', '\r', '?', '\xE', '\x41', 
		'\xF', '\x43', '\x10', '\x3', '\x2', '\x16', '\x4', '\x2', '\x45', '\x45', 
		'\x65', '\x65', '\x4', '\x2', 'T', 'T', 't', 't', '\x4', '\x2', 'G', 'G', 
		'g', 'g', '\x4', '\x2', '\x43', '\x43', '\x63', '\x63', '\x4', '\x2', 
		'\x44', '\x44', '\x64', '\x64', '\x4', '\x2', 'V', 'V', 'v', 'v', '\x4', 
		'\x2', 'Q', 'Q', 'q', 'q', '\x4', '\x2', 'K', 'K', 'k', 'k', '\x4', '\x2', 
		'P', 'P', 'p', 'p', '\x4', '\x2', 'M', 'M', 'm', 'm', '\x4', '\x2', 'Y', 
		'Y', 'y', 'y', '\x4', '\x2', 'I', 'I', 'i', 'i', '\x4', '\x2', 'W', 'W', 
		'w', 'w', '\x4', '\x2', '\x46', '\x46', '\x66', '\x66', '\x4', '\x2', 
		'U', 'U', 'u', 'u', '\x4', '\x2', 'R', 'R', 'r', 'r', '\x3', '\x2', '\x63', 
		'|', '\x3', '\x2', '\x43', '\\', '\x3', '\x2', '\x32', ';', '\x4', '\x2', 
		'\v', '\v', '\"', '\"', '\x2', '\xB8', '\x2', ')', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '+', '\x3', '\x2', '\x2', '\x2', '\x2', '-', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '/', '\x3', '\x2', '\x2', '\x2', '\x2', '\x31', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x33', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x35', '\x3', '\x2', '\x2', '\x2', '\x2', '\x37', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x39', '\x3', '\x2', '\x2', '\x2', '\x2', ';', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '=', '\x3', '\x2', '\x2', '\x2', '\x2', '?', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x41', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x43', '\x3', '\x2', '\x2', '\x2', '\x3', '\x45', '\x3', '\x2', '\x2', 
		'\x2', '\x5', 'G', '\x3', '\x2', '\x2', '\x2', '\a', 'I', '\x3', '\x2', 
		'\x2', '\x2', '\t', 'K', '\x3', '\x2', '\x2', '\x2', '\v', 'M', '\x3', 
		'\x2', '\x2', '\x2', '\r', 'O', '\x3', '\x2', '\x2', '\x2', '\xF', 'Q', 
		'\x3', '\x2', '\x2', '\x2', '\x11', 'S', '\x3', '\x2', '\x2', '\x2', '\x13', 
		'U', '\x3', '\x2', '\x2', '\x2', '\x15', 'W', '\x3', '\x2', '\x2', '\x2', 
		'\x17', 'Y', '\x3', '\x2', '\x2', '\x2', '\x19', '[', '\x3', '\x2', '\x2', 
		'\x2', '\x1B', ']', '\x3', '\x2', '\x2', '\x2', '\x1D', '_', '\x3', '\x2', 
		'\x2', '\x2', '\x1F', '\x61', '\x3', '\x2', '\x2', '\x2', '!', '\x63', 
		'\x3', '\x2', '\x2', '\x2', '#', '\x65', '\x3', '\x2', '\x2', '\x2', '%', 
		'g', '\x3', '\x2', '\x2', '\x2', '\'', 'i', '\x3', '\x2', '\x2', '\x2', 
		')', 'k', '\x3', '\x2', '\x2', '\x2', '+', 'r', '\x3', '\x2', '\x2', '\x2', 
		'-', 'w', '\x3', '\x2', '\x2', '\x2', '/', '}', '\x3', '\x2', '\x2', '\x2', 
		'\x31', '\x82', '\x3', '\x2', '\x2', '\x2', '\x33', '\x86', '\x3', '\x2', 
		'\x2', '\x2', '\x35', '\x8A', '\x3', '\x2', '\x2', '\x2', '\x37', '\x8F', 
		'\x3', '\x2', '\x2', '\x2', '\x39', '\x94', '\x3', '\x2', '\x2', '\x2', 
		';', '\x9A', '\x3', '\x2', '\x2', '\x2', '=', '\x9F', '\x3', '\x2', '\x2', 
		'\x2', '?', '\xAA', '\x3', '\x2', '\x2', '\x2', '\x41', '\xAF', '\x3', 
		'\x2', '\x2', '\x2', '\x43', '\xB4', '\x3', '\x2', '\x2', '\x2', '\x45', 
		'\x46', '\t', '\x2', '\x2', '\x2', '\x46', '\x4', '\x3', '\x2', '\x2', 
		'\x2', 'G', 'H', '\t', '\x3', '\x2', '\x2', 'H', '\x6', '\x3', '\x2', 
		'\x2', '\x2', 'I', 'J', '\t', '\x4', '\x2', '\x2', 'J', '\b', '\x3', '\x2', 
		'\x2', '\x2', 'K', 'L', '\t', '\x5', '\x2', '\x2', 'L', '\n', '\x3', '\x2', 
		'\x2', '\x2', 'M', 'N', '\t', '\x6', '\x2', '\x2', 'N', '\f', '\x3', '\x2', 
		'\x2', '\x2', 'O', 'P', '\t', '\a', '\x2', '\x2', 'P', '\xE', '\x3', '\x2', 
		'\x2', '\x2', 'Q', 'R', '\t', '\b', '\x2', '\x2', 'R', '\x10', '\x3', 
		'\x2', '\x2', '\x2', 'S', 'T', '\t', '\t', '\x2', '\x2', 'T', '\x12', 
		'\x3', '\x2', '\x2', '\x2', 'U', 'V', '\t', '\n', '\x2', '\x2', 'V', '\x14', 
		'\x3', '\x2', '\x2', '\x2', 'W', 'X', '\t', '\v', '\x2', '\x2', 'X', '\x16', 
		'\x3', '\x2', '\x2', '\x2', 'Y', 'Z', '\t', '\f', '\x2', '\x2', 'Z', '\x18', 
		'\x3', '\x2', '\x2', '\x2', '[', '\\', '\t', '\r', '\x2', '\x2', '\\', 
		'\x1A', '\x3', '\x2', '\x2', '\x2', ']', '^', '\t', '\xE', '\x2', '\x2', 
		'^', '\x1C', '\x3', '\x2', '\x2', '\x2', '_', '`', '\t', '\xF', '\x2', 
		'\x2', '`', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x61', '\x62', '\t', 
		'\x10', '\x2', '\x2', '\x62', ' ', '\x3', '\x2', '\x2', '\x2', '\x63', 
		'\x64', '\t', '\x11', '\x2', '\x2', '\x64', '\"', '\x3', '\x2', '\x2', 
		'\x2', '\x65', '\x66', '\t', '\x12', '\x2', '\x2', '\x66', '$', '\x3', 
		'\x2', '\x2', '\x2', 'g', 'h', '\t', '\x13', '\x2', '\x2', 'h', '&', '\x3', 
		'\x2', '\x2', '\x2', 'i', 'j', '\t', '\x14', '\x2', '\x2', 'j', '(', '\x3', 
		'\x2', '\x2', '\x2', 'k', 'l', '\x5', '\x3', '\x2', '\x2', 'l', 'm', '\x5', 
		'\x5', '\x3', '\x2', 'm', 'n', '\x5', '\a', '\x4', '\x2', 'n', 'o', '\x5', 
		'\t', '\x5', '\x2', 'o', 'p', '\x5', '\r', '\a', '\x2', 'p', 'q', '\x5', 
		'\a', '\x4', '\x2', 'q', '*', '\x3', '\x2', '\x2', '\x2', 'r', 's', '\x5', 
		'\x3', '\x2', '\x2', 's', 't', '\x5', '\xF', '\b', '\x2', 't', 'u', '\x5', 
		'\x11', '\t', '\x2', 'u', 'v', '\x5', '\x13', '\n', '\x2', 'v', ',', '\x3', 
		'\x2', '\x2', '\x2', 'w', 'x', '\x5', '\r', '\a', '\x2', 'x', 'y', '\x5', 
		'\x5', '\x3', '\x2', 'y', 'z', '\x5', '\t', '\x5', '\x2', 'z', '{', '\x5', 
		'\x3', '\x2', '\x2', '{', '|', '\x5', '\x15', '\v', '\x2', '|', '.', '\x3', 
		'\x2', '\x2', '\x2', '}', '~', '\x5', '\x19', '\r', '\x2', '~', '\x7F', 
		'\x5', '\x1B', '\xE', '\x2', '\x7F', '\x80', '\x5', '\x11', '\t', '\x2', 
		'\x80', '\x81', '\x5', '\x1D', '\xF', '\x2', '\x81', '\x30', '\x3', '\x2', 
		'\x2', '\x2', '\x82', '\x83', '\x5', '\v', '\x6', '\x2', '\x83', '\x84', 
		'\x5', '\t', '\x5', '\x2', '\x84', '\x85', '\x5', '\x19', '\r', '\x2', 
		'\x85', '\x32', '\x3', '\x2', '\x2', '\x2', '\x86', '\x87', '\x5', '\t', 
		'\x5', '\x2', '\x87', '\x88', '\x5', '\x1D', '\xF', '\x2', '\x88', '\x89', 
		'\x5', '\x1D', '\xF', '\x2', '\x89', '\x34', '\x3', '\x2', '\x2', '\x2', 
		'\x8A', '\x8B', '\x5', '\v', '\x6', '\x2', '\x8B', '\x8C', '\x5', '\t', 
		'\x5', '\x2', '\x8C', '\x8D', '\x5', '\x19', '\r', '\x2', '\x8D', '\x8E', 
		'\x5', '\x1F', '\x10', '\x2', '\x8E', '\x36', '\x3', '\x2', '\x2', '\x2', 
		'\x8F', '\x90', '\x5', '\x1F', '\x10', '\x2', '\x90', '\x91', '\x5', '\r', 
		'\a', '\x2', '\x91', '\x92', '\x5', '\xF', '\b', '\x2', '\x92', '\x93', 
		'\x5', '!', '\x11', '\x2', '\x93', '\x38', '\x3', '\x2', '\x2', '\x2', 
		'\x94', '\x95', '\x5', '\r', '\a', '\x2', '\x95', '\x96', '\x5', '\xF', 
		'\b', '\x2', '\x96', ':', '\x3', '\x2', '\x2', '\x2', '\x97', '\x9B', 
		'\x5', '#', '\x12', '\x2', '\x98', '\x9B', '\x5', '%', '\x13', '\x2', 
		'\x99', '\x9B', '\a', '/', '\x2', '\x2', '\x9A', '\x97', '\x3', '\x2', 
		'\x2', '\x2', '\x9A', '\x98', '\x3', '\x2', '\x2', '\x2', '\x9A', '\x99', 
		'\x3', '\x2', '\x2', '\x2', '\x9B', '\x9C', '\x3', '\x2', '\x2', '\x2', 
		'\x9C', '\x9A', '\x3', '\x2', '\x2', '\x2', '\x9C', '\x9D', '\x3', '\x2', 
		'\x2', '\x2', '\x9D', '<', '\x3', '\x2', '\x2', '\x2', '\x9E', '\xA0', 
		'\t', '\x15', '\x2', '\x2', '\x9F', '\x9E', '\x3', '\x2', '\x2', '\x2', 
		'\xA0', '\xA1', '\x3', '\x2', '\x2', '\x2', '\xA1', '\x9F', '\x3', '\x2', 
		'\x2', '\x2', '\xA1', '\xA2', '\x3', '\x2', '\x2', '\x2', '\xA2', '\xA3', 
		'\x3', '\x2', '\x2', '\x2', '\xA3', '\xA4', '\b', '\x1F', '\x2', '\x2', 
		'\xA4', '>', '\x3', '\x2', '\x2', '\x2', '\xA5', '\xA7', '\a', '\xF', 
		'\x2', '\x2', '\xA6', '\xA5', '\x3', '\x2', '\x2', '\x2', '\xA6', '\xA7', 
		'\x3', '\x2', '\x2', '\x2', '\xA7', '\xA8', '\x3', '\x2', '\x2', '\x2', 
		'\xA8', '\xAB', '\a', '\f', '\x2', '\x2', '\xA9', '\xAB', '\a', '\xF', 
		'\x2', '\x2', '\xAA', '\xA6', '\x3', '\x2', '\x2', '\x2', '\xAA', '\xA9', 
		'\x3', '\x2', '\x2', '\x2', '\xAB', '\xAC', '\x3', '\x2', '\x2', '\x2', 
		'\xAC', '\xAA', '\x3', '\x2', '\x2', '\x2', '\xAC', '\xAD', '\x3', '\x2', 
		'\x2', '\x2', '\xAD', '@', '\x3', '\x2', '\x2', '\x2', '\xAE', '\xB0', 
		'\x5', '\'', '\x14', '\x2', '\xAF', '\xAE', '\x3', '\x2', '\x2', '\x2', 
		'\xB0', '\xB1', '\x3', '\x2', '\x2', '\x2', '\xB1', '\xAF', '\x3', '\x2', 
		'\x2', '\x2', '\xB1', '\xB2', '\x3', '\x2', '\x2', '\x2', '\xB2', '\x42', 
		'\x3', '\x2', '\x2', '\x2', '\xB3', '\xB5', '\x5', '\'', '\x14', '\x2', 
		'\xB4', '\xB3', '\x3', '\x2', '\x2', '\x2', '\xB5', '\xB6', '\x3', '\x2', 
		'\x2', '\x2', '\xB6', '\xB4', '\x3', '\x2', '\x2', '\x2', '\xB6', '\xB7', 
		'\x3', '\x2', '\x2', '\x2', '\xB7', '\xB9', '\x3', '\x2', '\x2', '\x2', 
		'\xB8', '\xBA', '\a', '\x30', '\x2', '\x2', '\xB9', '\xB8', '\x3', '\x2', 
		'\x2', '\x2', '\xB9', '\xBA', '\x3', '\x2', '\x2', '\x2', '\xBA', '\xBE', 
		'\x3', '\x2', '\x2', '\x2', '\xBB', '\xBD', '\x5', '\'', '\x14', '\x2', 
		'\xBC', '\xBB', '\x3', '\x2', '\x2', '\x2', '\xBD', '\xC0', '\x3', '\x2', 
		'\x2', '\x2', '\xBE', '\xBC', '\x3', '\x2', '\x2', '\x2', '\xBE', '\xBF', 
		'\x3', '\x2', '\x2', '\x2', '\xBF', '\x44', '\x3', '\x2', '\x2', '\x2', 
		'\xC0', '\xBE', '\x3', '\x2', '\x2', '\x2', '\r', '\x2', '\x9A', '\x9C', 
		'\xA1', '\xA6', '\xAA', '\xAC', '\xB1', '\xB6', '\xB9', '\xBE', '\x3', 
		'\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
