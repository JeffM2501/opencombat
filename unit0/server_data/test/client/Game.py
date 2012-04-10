def InitGame (gameType):
    icoTank = Game.AddAvatar("models/icotank/");
    icoTank.AddTeamSkin("models/icotank/blue.png");
    icoTank.AddTeamSkin("models/icotank/purple.png");
    icoTank.AddTeamSkin("models/icotank/red.png");
    icoTank.AddTeamSkin("models/icotank/yellow.png");
    return True;