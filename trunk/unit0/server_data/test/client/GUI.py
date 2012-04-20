def LoadHud (HUD):
	chatBox = HUD.NewElement("chatbox","chatbox");
    chatBox.alignment = PannelElement.Alignmnet.RightBottom;
    chatBox.origin = PannelElement.Alignmnet.RightBottom;
    chatBox.SetPos(0,0);
    chatBox.SetSize(300,250);
    chatBox.color = PannelElement.ElementColor.White;
    chatBox.enabled = true;
    GUIRenderer.LoadElement(chatBox);
    return True;