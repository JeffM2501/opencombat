def SetGameInfo (info):
    info.GameStyle = "Testing_Game"
    tankInfo = info.NewOption("tank",0);
    tankInfo.Options.Add("Standard");
    return True