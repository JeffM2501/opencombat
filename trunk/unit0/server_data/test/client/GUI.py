def LoadHud (HUD):
	chatFrame = HUD.NewElement("chatbox","chatbox");
	chatFrame.alignment = HUD.AlignRightBottom;
	chatFrame.origin = HUD.AlignRightBottom;
	chatFrame.SetPosition(0,0);
	chatFrame.SetSize(500,128);
	chatFrame.enabled = True;

	chatList = chatFrame.NewChild("chatWindow","chatWindow");
	chatList.updateFunction = "CHAT_LOG";
	chatList.alignment = HUD.AlignLeftBottom;
	chatList.origin = HUD.AlignLeftBottom;
	chatList.SetPosition(15,35);
	chatList.SetSize(460,80);
	chatList.enabled = True;

	chatEntry = chatFrame.NewChild("chatEntry","chatFontTextEdit");
	chatEntry.updateFunction = "CHAT_ENTRY";
	chatEntry.alignment = HUD.AlignLeftBottom;
	chatEntry.origin = HUD.AlignLeftBottom;
	chatEntry.SetPosition(15,2);
	chatEntry.SetSize(460,30);
	chatEntry.enabled = True;

	HUD.LoadElement(chatFrame);

	radarFrame = HUD.NewElement("radar","image");
	radarFrame.alignment = HUD.AlignLeftBottom;
	radarFrame.origin = HUD.AlignLeftBottom;
	radarFrame.SetPosition(0,0);
	radarFrame.SetSize(256,256);
	radarFrame.enabled = True;
	radarFrame.text = "ui/radar.png";
	HUD.LoadElement(radarFrame);

	playerFrame = HUD.NewElement("stats","image");
	playerFrame.alignment = HUD.AlignRightTop;
	playerFrame.origin = HUD.AlignRightTop;
	playerFrame.SetPosition(0,0);
	playerFrame.SetSize(256,128);
	playerFrame.enabled = True;
	playerFrame.text = "ui/playerInfo.png";
	HUD.LoadElement(playerFrame);

	infoFrame = HUD.NewElement("info","image");
	infoFrame.alignment = HUD.AlignLeftTop;
	infoFrame.origin = HUD.AlignLeftTop;
	infoFrame.SetPosition(0,0);
	infoFrame.SetSize(256,64);
	infoFrame.enabled = True;
	infoFrame.text = "ui/short_upper_left.png";

	fpsItem = infoFrame.NewChild("fps","statsFontLabel");
	fpsItem.alignment = HUD.AlignLeftTop;
	fpsItem.origin = HUD.AlignLeftTop;
	fpsItem.SetPosition(5,-5);
	fpsItem.text = "FPS:";
	fpsItem.enabled = True;

	fpsItem = infoFrame.NewChild("fps","statsFontLabel");
	fpsItem.updateFunction = "FPS";
	fpsItem.alignment = HUD.AlignLeftTop;
	fpsItem.origin = HUD.AlignLeftTop;
	fpsItem.SetPosition(65,-5);
	fpsItem.enabled = True;

	HUD.LoadElement(infoFrame);
	return True;
