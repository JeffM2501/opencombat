

def LoadHud (HUD):
    chatFrame = HUD.NewElement("chatbox","chatbox");
    chatFrame.alignment = HUD.AlignRightBottom;
    chatFrame.origin = HUD.AlignRightBottom;
    chatFrame.SetPosition(0,0);
    chatFrame.SetSize(500,250);
    chatFrame.enabled = True;
    
    chatList = chatFrame.NewChild("chatWindow","chatWindow");
    chatList.updateFunction = "CHAT_LOG";
    chatList.alignment = HUD.AlignLeftBottom;
    chatList.origin = HUD.AlignLeftBottom;
    chatList.SetPosition(15,35);
    chatList.SetSize(460,200);
    chatList.enabled = True;

    HUD.LoadElement(chatFrame);
    return True;
