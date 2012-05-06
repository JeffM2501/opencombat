
def GetBotCount ():
    return 1;

def NewRobot (player, botID):
    player.BotID = botID;
    return True;

def HandleChat (player, message, server):
    if message.From != player.UID:
        server.SendInstanceChat(player,"Heyya");
    return;