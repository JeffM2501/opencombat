

def LoadHud (HUD):
    chatBox = HUD.NewElement("chatbox","chatbox");
    chatBox.alignment = HUD.AlignRightBottom;
    chatBox.origin = HUD.AlignRightBottom;
    chatBox.SetPosition(0,0);
    chatBox.SetSize(500,250);
    chatBox.enabled = True;
    HUD.LoadElement(chatBox);
    return True;
