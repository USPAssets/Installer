/**
 *  Undertale Spaghetti Project
 *  DELTARUNE Chapter 1&2 Translation script
 *
 *  @version 1.4
 *  @author USP
 */

/** usings */
using UndertaleModLib.Util; // TextureWorker
using System.Text; // StringBuilder
using System.Xml; // XmlDocument
using System.Text.RegularExpressions; // Regex
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UndertaleModLib.Util;
using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UndertaleModLib.Util;


string g_AssetsPath;
string g_GameFolder;
float g_CurrentProgress;
float g_TotalProgress;

/** GMX parser stuff */

bool GmxBool(string innerText) {
	int result = 0;
	if (!int.TryParse(innerText, out result)) {
		try {
			result = Convert.ToBoolean(innerText) ? -1 : 0;
		}
		catch {
			ScriptError(string.Format("Failed to parse GMX value '{0}' as a bool.", innerText), "GMX Error");
		}
	}
	
	return result != 0;
}

int GmxInt(string innerText) {
	int result = 0;
	if (!int.TryParse(innerText, out result)) {
		// TODO: exception?
		ScriptError(string.Format("Failed to parse GMX value '{0}' as an int.", innerText), "GMX Error");
	}
	
	return result;
}

string GmxString(string innerText) {
	// TODO: sanitize string from XML stuff
	if (innerText is null) {
		ScriptError(string.Format("The GMX string '{0}' is null.", innerText), "GMX Error");
	}
	
	return innerText;
}

void DoProgress(string progressName) {
	++g_CurrentProgress;
	UpdateProgressBar(
		progressName,
		"...",
		MathF.Floor((g_CurrentProgress / g_TotalProgress) * 100.0f), 
		100.0f
	);
}

void SmartFontReplace(string origName, string scapegoatName) {
	/* var path = Path.Combine(g_AssetsPath, "Fonts"); */

	/* string fontGmx = Path.Combine(path, origName + ".font.gmx"); */
	/* string fontPng = null; */
	/*  */
	/* UndertaleFont ufont = Data.Fonts.ByName(origName); */
	/* if (ufont is null) { */
	/* 	ScriptError(string.Format("Font '{0}' was not found in the game.", origName), "Font Error"); */
	/* 	return; */
	/* } */
	/*  */
	/* XmlDocument xdoc = new XmlDocument(); */
	/* xdoc.Load(fontGmx); */
	/*  */
	//* int gfirst = int.MaxValue /* def: 32 */; */
	//* int glast = int.MinValue /* def: 127 */; */
	/*  */
	//* List<Tuple<int/*:mychar*/, int/*:other*/, int/*:amount*/>> kpairslist = new List<Tuple<int/*:mychar*/, int/*:other*/, int/*:amount*/>>(); */
	/*  */
	/* foreach (XmlNode xnode in xdoc.SelectNodes("/font/*")) { */
	/* 	string xname = xnode.Name; */
	/* 	switch (xname) { */
	/* 		default: { */
	/* 			ScriptError(string.Format("Unknown entry '{0}' found in Font GMX.", xname), "GMX Error"); */
	/* 			return; */
	/* 		} */
	/* 		 */
	/* 		case "name": { */
	/* 			string gmxfontname = GmxString(xnode.InnerText); */
	/* 			ufont.DisplayName.Content = gmxfontname; */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "renderhq": { */
	/* 			// this is actually unused but still parsed to prevent errors. */
	/* 			bool buserenderhq = GmxBool(xnode.InnerText); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "includeTTF": { */
	/* 			// this *may* be useful if the script will render all textures in standalone. */
	/* 			bool bincludettf = GmxBool(xnode.InnerText); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "TTFName": { */
	/* 			// see includeTTF, if includeTTF is true, usually this is always set. */
	/* 			string bttffilepath = GmxString(xnode.InnerText); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "texgroups": { */
	/* 			// just parse them but not actually do anything. */
	/* 			// since texgroups are already handled by the game. */
	/* 			foreach (XmlNode xtexgroupnode in xnode.ChildNodes) { */
	/* 				string texstring = GmxString(xtexgroupnode.Name).Substring("texgroup".Length); */
	/* 				int texindex = GmxInt(texstring); */
	/* 				int texassetid = GmxInt(xtexgroupnode.InnerText); */
	/* 				// TextureGroups[texindex] = texassetid; */
	/* 			} */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "kerningPairs": { */
	/* 			foreach (XmlNode xkern in xnode.SelectNodes("pair")) { */
	/* 				int knum = GmxInt(xkern.Attributes["first"].Value); */
	/* 				int kother = GmxInt(xkern.Attributes["second"].Value); */
	/* 				int kamount = GmxInt(xkern.Attributes["amount"].Value); */
	//* 				kpairslist.Add(new Tuple<int/*:mychar*/, int/*:other*/, int/*:amount*/>(knum/*mychar*/, kother/*other*/, kamount/*amount*/)); */
	/* 			} */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "italic": { */
	/* 			ufont.Italic = GmxBool(xnode.InnerText); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "bold": { */
	/* 			ufont.Bold = GmxBool(xnode.InnerText); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "size": { */
	/* 			ufont.EmSize = checked((uint)GmxInt(xnode.InnerText)); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "charset": { */
	/* 			ufont.Charset = checked((byte)GmxInt(xnode.InnerText)); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "aa": { */
	/* 			ufont.AntiAliasing = checked((byte)GmxInt(xnode.InnerText)); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "image": { */
	/* 			fontPng = Path.Combine(path, GmxString(xnode.InnerText)); */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "ranges": { */
	/* 			foreach (XmlNode xrange in xnode.ChildNodes) { */
	/* 				string[] rarr = GmxString(xrange.InnerText).Split(','); */
	//* 				int ra = GmxInt(rarr[0/*first*/]); */
	//* 				int rb = GmxInt(rarr[1/*last*/]); */
	/* 				gfirst = Math.Min(gfirst, ra); */
	/* 				glast = Math.Max(glast, rb); */
	/* 			} */
	/* 			break; */
	/* 		} */
	/* 		 */
	/* 		case "glyphs": { */
	/* 			ufont.Glyphs.Clear(); */
	/* 			foreach (XmlNode xglyph in xnode.SelectNodes("glyph")) { */
	/* 				int gchar = GmxInt(xglyph.Attributes["character"].Value); */
	/* 				int gx = GmxInt(xglyph.Attributes["x"].Value); */
	/* 				int gy = GmxInt(xglyph.Attributes["y"].Value); */
	/* 				int gw = GmxInt(xglyph.Attributes["w"].Value); */
	/* 				int gh = GmxInt(xglyph.Attributes["h"].Value); */
	/* 				int gshift = GmxInt(xglyph.Attributes["shift"].Value); */
	/* 				int goffset = GmxInt(xglyph.Attributes["offset"].Value); */
	/* 				ufont.Glyphs.Add(new UndertaleFont.Glyph() { */
	/* 					Character = checked((ushort)gchar), */
	/* 					SourceX = checked((ushort)gx), */
	/* 					SourceY = checked((ushort)gy), */
	/* 					SourceWidth = checked((ushort)gw), */
	/* 					SourceHeight = checked((ushort)gh), */
	/* 					Shift = checked((short)gshift), */
	/* 					Offset = checked((short)goffset), */
	/* 					Kerning = new UndertaleSimpleListShort<UndertaleFont.Glyph.GlyphKerning>() */
	/* 				}); */
	/* 			} */
	/* 			 */
	/* 			break; */
	/* 		} */
	/* 	} */
	/* } */
	/*  */
	/* // post process... */
	/* ufont.RangeStart = checked((ushort)gfirst); */
	/* ufont.RangeEnd = checked((ushort)glast); */
	/*  */
	/* foreach (var ktuple in kpairslist) { */
	/* 	foreach (var theglyph in ufont.Glyphs) { */
	//* 		if (ktuple.Item1/*:mychar*/ == theglyph.Character) { */
	/* 			// found our kerning pair: */
	/* 			theglyph.Kerning.Add(new UndertaleFont.Glyph.GlyphKerning() { */
	//* 				Other = checked((short)ktuple.Item2/*:other*/), */
	//* 				Amount = checked((short)ktuple.Item3/*:amount*/) */
	/* 			}); */
	/* 		} */
	/* 	} */
	/* } */
	/*  */
	/* // obtain font texture */
	/* var myFontTexture = TextureWorker.ReadImageFromFile(fontPng); */
	/*  */
	/* var targetFontTexture = ufont.Texture; */
	/*  */
	/* var doScapegoat = (!(scapegoatName is null)) && scapegoatName.Length > 0; */
	/* if (doScapegoat) { */
	/* 	ufont.Texture = Data.Fonts.ByName(scapegoatName).Texture; */
	/* 	targetFontTexture = ufont.Texture; */
	/* } */
	/*  */
	/* // ensure that we're not going to horribly break shit. */
	/* if (myFontTexture.Width  > targetFontTexture.SourceWidth */
	/* ||  myFontTexture.Height > targetFontTexture.SourceHeight) { */
	/* 	ScriptError("The font texture is LARGER than the game's.", "Font Import Error"); */
	/* } */
	/*  */
	/* // this will shrink down the texture if using scapegoat font */
	/* // or if the font happens to be larger. */
	/* targetFontTexture.SourceWidth = checked((ushort)myFontTexture.Width); */
	/* targetFontTexture.SourceHeight = checked((ushort)myFontTexture.Height); */
	/* targetFontTexture.TargetWidth = checked((ushort)myFontTexture.Width); */
	/* targetFontTexture.TargetHeight = checked((ushort)myFontTexture.Height); */
	/* targetFontTexture.BoundingWidth = checked((ushort)myFontTexture.Width); */
	/* targetFontTexture.BoundingHeight = checked((ushort)myFontTexture.Height); */
	/*  */
	/* // render the texture on top of the original one. */
	/* targetFontTexture.ReplaceTexture(myFontTexture, true); */
	/*  */
	// should be done?
}

void DoFonts() {
	DoProgress("Fonts");
	// null - the italian texture is equal to, or smaller.
	// otherwise, a japanese font is specified to abuse it's large AF texture.
	// (the JP texture will be wiped out, and italian font will be drawn at 0;0)
	SmartFontReplace("fnt_tinynoelle", null);
	SmartFontReplace("fnt_dotumche", "fnt_ja_dotumche"); // doesn't seem to fit... :(
	// okay now we HAVE to import this font.
	SmartFontReplace("fnt_mainbig", null);
	SmartFontReplace("fnt_main", null);
	SmartFontReplace("fnt_comicsans", null);
	SmartFontReplace("fnt_small", null);
}

void DoExportStrings() {
	Regex varNameExpr = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
	StringBuilder sb = new StringBuilder();
	
	foreach (var utstr in Data.Strings) {
		var asStr = utstr.Content;
		var doWrite = !varNameExpr.IsMatch(asStr);
		doWrite = doWrite
		&& !asStr.Contains("#define")
		&& !asStr.StartsWith("@@")
		&& !asStr.Contains("global.")
		&& !asStr.Contains("$$$temp")
		&& !asStr.Contains(".ogg")
		&& !asStr.Contains(".ini")
		&& !asStr.Contains(".json")
		&& !asStr.Contains(".txt")
		&& !asStr.Contains("// GameMaker")
		&& !asStr.Contains("Compatibility_Instances");
		
		if (doWrite) {
			// escape the newlines only
			asStr = asStr.Replace("\n", "\\n").Replace("\r", "\\r");
			sb.AppendLine(asStr);
		}
	}
	
	var sbs = sb.ToString();
	
	var itTXT = Path.Combine(g_AssetsPath, "stringLookup_IT.txt");
	var enTXT = Path.Combine(g_AssetsPath, "stringLookup_EN.txt");
	if (!File.Exists(itTXT) && !File.Exists(enTXT)) {
		File.WriteAllText(itTXT, sbs);
		File.WriteAllText(enTXT, sbs);
	}
	else {
		ScriptMessage("String files already exist, please delete all of them and run me again.");
	}	
}

void DoImportCh1Strings() {	
	var langFolder = Path.Combine(g_GameFolder, "lang");
	
	var langch1 = Path.Combine(langFolder, "lang_en_ch1.json");
	var langch1IT = Path.Combine(g_AssetsPath, "Strings", "lang_it_ch1.json");
	
	File.Copy(langch1IT, langch1, true);
}

void DoImportCh2Strings() {
	var stringsCh2IT = Path.Combine(g_AssetsPath, "Strings", "strings_ch2.txt");
	if (!File.Exists(stringsCh2IT)) {
		ScriptError("String file for Ch2 does not exist, strings will not be imported", "Strings Import Error");
	}
	
	using (StreamReader reader = new StreamReader(stringsCh2IT)) {
		foreach (var str in Data.Strings) {
			var line = reader.ReadLine();
			if (line != null) {
				str.Content = line.Replace("\\n", "\n").Replace("\\r", "\r");
			} else {
				ScriptError("String file for Ch2 has less lines than expected.", "Strings Import Error");
				return;
			}
		}
	}
}

void DoStrings()
{
    DoProgress("Strings");
    string stringsPath = PromptLoadFile("", "TXT files (*.txt)|*.txt|All files (*.*)|*.*");
    int file_length = 0;
    string line = "";
     using (StreamReader reader = new StreamReader(stringsPath))
    {
        while ((line = reader.ReadLine()) != null)
        {
            file_length += 1;
        }
    }

    int validStringsCount = 0;
    foreach (var str in Data.Strings)
    {
        if (str.Content.Contains("\n") || str.Content.Contains("\r"))
            continue;
        validStringsCount += 1;
    }

    if (file_length < validStringsCount)
    {
        ScriptError("ERROR 0: Unexpected end of file at line: " + file_length.ToString() + ". Expected file length was: " + validStringsCount.ToString() + ". No changes have been made.", "Error");
        return;
    }
    else if (file_length > validStringsCount)
    {
        ScriptError("ERROR 1: Line count exceeds expected count. Current count: " + file_length.ToString() + ". Expected count: " + validStringsCount.ToString() + ". No changes have been made.", "Error");
        return;
    }

    using (StreamReader reader = new StreamReader(stringsPath))
    {
        int line_no = 1;
        line = "";
        foreach (var str in Data.Strings)
        {
            if (str.Content.Contains("\n") || str.Content.Contains("\r"))
                continue;
            if (!((line = reader.ReadLine()) is not null))
            {
                ScriptError("ERROR 2: Unexpected end of file at line: " + line_no.ToString() + ". Expected file length was: " + validStringsCount.ToString() + ". No changes have been made.", "Error");
                return;
            }
            line_no += 1;
        }
    }

    using (StreamReader reader = new StreamReader(stringsPath))
    {
        int line_no = 1;
        line = "";
        foreach (var str in Data.Strings)
        {
            if (str.Content.Contains("\n") || str.Content.Contains("\r"))
                continue;
            //if ((line = reader.ReadLine()) is not null)
            if ((line =  reader.ReadLine()) != null)
                str.Content = line;
            else
            {
                ScriptError("ERROR 3: Unexpected end of file at line: " + line_no.ToString() + ". Expected file length was: " + validStringsCount.ToString() + ". All lines within the file have been applied. Please check for errors.", "Error");
                return;
            }
            line_no += 1;
        }
    }

	// uncomment this if you want...
    // DoExportStrings();
    //DoImportCh1Strings();
    //DoImportCh2Strings();
}

void DoSprites() {
	DoProgress("Sprites");
	
	/* var spritesFolder = Path.Combine(g_AssetsPath, "Sprites");
	if (!Directory.Exists(spritesFolder)) {
		ScriptError("Sprite folder does not exist, they will not be imported.", "Sprite Import Error");
		return;
	}
	
	var did = 0;
	
	foreach (var spr in Data.Backgrounds) {
		if (spr is null) continue;
		if (spr.Texture is null) continue;
		
		var pngpath = Path.Combine(spritesFolder, spr.Name.Content + ".png");
		if (!File.Exists(pngpath)) continue;
		
		// do the replacement:
		var sprt = TextureWorker.ReadImageFromFile(pngpath);
		
		spr.Texture.ReplaceTexture(sprt, true);
		++did;
		ScriptMessage("Found tileset = " + spr.Name.Content);
		
		try {
			sprt.Dispose();
		} catch {
			// ignore.
		}
	}
	
	foreach (var spr in Data.Sprites) {
		if (spr is null) continue;
		
		// needs to be indexed!
		for (var f = 0; f < spr.Textures.Count; ++f) {
			if (spr.Textures[f] is null) continue;
			
			var pngpath = Path.Combine(spritesFolder, spr.Name.Content + "_" + f.ToString() + ".png");
			if (!File.Exists(pngpath)) continue;
			
			// do the replacement:
			var sprt = TextureWorker.ReadImageFromFile(pngpath);
			
			spr.Textures[f].Texture.ReplaceTexture(sprt, true);
			++did;
			
			try {
				sprt.Dispose();
			} catch {
				// ignore.
			}
		}
	}
	
	ScriptMessage("Replaced " + did.ToString() + " sprites."); */
}

void DoSoundReplace(string wavPath) {
	/* var fn = Path.GetFileNameWithoutExtension(wavPath);
	
	foreach (var snd in Data.Sounds) {
		if (snd is null) continue;
		
		if (snd.Name.Content == fn) {
			var myid = snd.AudioID;
			var fbytes = File.ReadAllBytes(wavPath);
			Data.EmbeddedAudio[myid].Data = fbytes;
			ScriptMessage("Sound found " + fn);
			return;
		}
	} */
}

void DoSounds() {
	DoProgress("Sounds");
	
	/* var sndFolder = Path.Combine(g_AssetsPath, "Sounds");
	if (!Directory.Exists(sndFolder)) {
		ScriptError("Sound folder does not exist, they will not be imported.", "Sound Import Error");
		return;
	}
	
	var dfPath = Path.Combine(sndFolder, "dontforget_IT.ogg");
	
	var musFolderO = Path.Combine(g_GameFolder, "mus");
	var dfPathO = Path.Combine(musFolderO, "dontforget.ogg");
	
	// delete english dontforget
	File.Delete(dfPathO);
	// copy new one
	File.Copy(dfPath, dfPathO);
	
	// nik wuz here
	DoSoundReplace(Path.Combine(sndFolder, "snd_joker_anything_ch1.wav"));
	DoSoundReplace(Path.Combine(sndFolder, "snd_joker_byebye_ch1.wav"));
	DoSoundReplace(Path.Combine(sndFolder, "snd_joker_chaos_ch1.wav"));
	DoSoundReplace(Path.Combine(sndFolder, "snd_joker_metamorphosis_ch1.wav"));
	DoSoundReplace(Path.Combine(sndFolder, "snd_joker_neochaos_ch1.wav")); */
}

UndertaleCode GetCode(string name)
{
    /* foreach (var c in Data.Code)
    {
        if (c.Name.Content == name) return c;
    }
    */

    throw new Exception("Unable to find code! " + name); 
}

void DoCode() {
	/* DoProgress("Code");
	
	// keyboard puzzle.
	
	// obj_ch2_keyboardpuzzle_tile: Variable Definitions
	GetCode("gml_Object_obj_ch2_keyboardpuzzle_tile_PreCreate_0").Instructions
		[2].Value = MakeString("M"); // push.s "M";
	
	// room_dw_cyber_keyboard_puzzle_1: Variable Definitions
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_1_1_PreCreate").Instructions
		[0].Value = MakeString("E"); // push.s "E";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_1_2_PreCreate").Instructions
		[0].Value = MakeString("L"); // push.s "L";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_1_3_PreCreate").Instructions
		[0].Value = MakeString("A"); // push.s "A";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_1_4_PreCreate").Instructions
		[0].Value = MakeString("A"); // push.s "A";
	
	// room_dw_cyber_keyboard_puzzle_2: Variable Definitions
	var instrsA = new List<UndertaleInstruction>() {
		// push.s "A";
		new UndertaleInstruction() {
			Value = MakeString("A"),
			Kind = UndertaleInstruction.Opcode.Push,
			Type1 = UndertaleInstructionUtil.FromOpcodeParam("s"),
			Type2 = UndertaleInstructionUtil.FromOpcodeParam("d"),
			TypeInst = UndertaleInstruction.InstanceType.Undefined
		},
		// pop.v.s self.myString;
		new UndertaleInstruction() {
			Destination = MakeVarRef("myString"),
			Kind = UndertaleInstruction.Opcode.Pop,
			Type1 = UndertaleInstructionUtil.FromOpcodeParam("v"),
			Type2 = UndertaleInstructionUtil.FromOpcodeParam("s"),
			TypeInst = UndertaleInstruction.InstanceType.Self
		}
	};
	
	// self.myString = "A";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_1_PreCreate")
		.Append(instrsA);
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_2_PreCreate").Instructions
		[0].Value = MakeString("N"); // push.s "N";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_3_PreCreate").Instructions
		[0].Value = MakeString("A"); // push.s "A";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_4_PreCreate").Instructions
		[0].Value = MakeString("N"); // push.s "N";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_5_PreCreate").Instructions
		[0].Value = MakeString("C"); // push.s "C";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_6_PreCreate").Instructions
		[0].Value = MakeString("S"); // push.s "S";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_7_PreCreate").Instructions
		[0].Value = MakeString("O"); // push.s "O";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_8_PreCreate").Instructions
		[0].Value = MakeString("E"); // push.s "E";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_9_PreCreate").Instructions
		[0].Value = MakeString("M"); // push.s "M";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_10_PreCreate").Instructions
		[0].Value = MakeString("T"); // push.s "T";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_11_PreCreate").Instructions
		[0].Value = MakeString("C"); // push.s "C";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_12_PreCreate").Instructions
		[0].Value = MakeString("S"); // push.s "S";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_13_PreCreate").Instructions
		[0].Value = MakeString("O"); // push.s "O";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_14_PreCreate").Instructions
		[0].Value = MakeString("T"); // push.s "T";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_15_PreCreate").Instructions
		[0].Value = MakeString("D"); // push.s "D";
		
	
	var instrsI = new List<UndertaleInstruction>() {
		// push.s "I";
		new UndertaleInstruction() {
			Value = MakeString("I"),
			Kind = UndertaleInstruction.Opcode.Push,
			Type1 = UndertaleInstructionUtil.FromOpcodeParam("s"),
			Type2 = UndertaleInstructionUtil.FromOpcodeParam("d"),
			TypeInst = UndertaleInstruction.InstanceType.Undefined
		},
		// pop.v.s self.myString;
		new UndertaleInstruction() {
			Destination = MakeVarRef("myString"),
			Kind = UndertaleInstruction.Opcode.Pop,
			Type1 = UndertaleInstructionUtil.FromOpcodeParam("v"),
			Type2 = UndertaleInstructionUtil.FromOpcodeParam("s"),
			TypeInst = UndertaleInstruction.InstanceType.Self
		}
	};
	
	// // self.myString = "I";
	GetCode("gml_RoomCC_room_dw_cyber_keyboard_puzzle_2_16_PreCreate")
		.Append(instrsI); */
	
	// end.
}

void Done() {
	HideProgressBar();
	ScriptMessage("Done. PLEASE save and overwrite your file (Ctrl+S) and run the game!");
}

void ScriptEntry() {
	// TODO: translate script to le pasta?
	// only work with actual data files please.
	EnsureDataLoaded();
	
	g_CurrentProgress = 0.0f;
	g_TotalProgress = 7.0f; // UPDATE THIS
	
	if (ScriptPath is null) {
		ScriptError("This script can only be ran as a file on disk.", "Assets Error");
		return;
	}
	
	g_AssetsPath = Path.Combine(Path.GetDirectoryName(ScriptPath), "Assets");
	if (!Directory.Exists(g_AssetsPath)) {
		ScriptError("Translation assets directory does not exist. Please unpack the whole zip archive!", "Assets Error");
		return;
	}
	
	g_GameFolder = Path.GetDirectoryName(FilePath);
	// check for `mus` folder's presence (needed for sound replacement)
	if (!Directory.Exists(Path.Combine(g_GameFolder, "mus"))) {
		ScriptError("Game's folder does not seem to be a valid one.", "Assets Error");
		return;
	}
	
	var gameName = Data.GeneralInfo?.Name?.Content;
	if (gameName != "DELTARUNE") {
		ScriptError("This game is not the 'Chapter 1&2' demo.", "Game Error");
		return;
	}
	
	var doPatch = true;
	// the API says nothing against that :p
	//if (DummyString() != "<usp_installer>") doPatch = ScriptQuestion("This will modify your .win file, proceed?");
	//if (!doPatch) return;
	
	DoStrings();
	DoSprites();
	DoFonts();
	DoSounds();
	DoCode();
	
	Done();
}
public string PromptLoadFile(string defaultExt, string filter)
{
    string path;
    FileInfo fileInfo;
    do
    {
        Console.WriteLine("Please enter a path (or drag and drop) to a valid file:");
        Console.Write("Path: ");
        path = RemoveQuotes(Console.ReadLine());
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        fileInfo = new FileInfo(path);
    }
    while (fileInfo.Exists);

    return path;
}

 private static string RemoveQuotes(string s)

{
    return s.Trim('"', '\'');
}

/** the script starts here */
ScriptEntry();
