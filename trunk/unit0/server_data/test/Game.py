def SetGameInfo (info):
    info.GameStyle = "Testing_Game";
    tankInfo = info.NewOption("tank",0);
    tankInfo.Options.Add("Standard");
    info.Avatars.Add("IcoTank2");
    return True;

def SetNewPlayerInfo (player):
    player.AvatarID = 0;
    return True;

def GetPlayerSpawn (player):
    return ServerAPI.GetStandardSpawn();