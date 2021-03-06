﻿using SS14.Server.Interfaces.GameObjects;
using SS14.Server.Interfaces.Player;
using SS14.Server.Interfaces.Round;
using SS14.Shared;
using SS14.Shared.GameObjects;
using SS14.Shared.IoC;

namespace SS14.Server.GameObjects
{
    public class BasicActorComponent : Component, IActorComponent
    {
        public override string Name => "BasicActor";
        public IPlayerSession playerSession { get; internal set; }

        public override ComponentReplyMessage ReceiveMessage(object sender, ComponentMessageType type,
                                                             params object[] list)
        {
            ComponentReplyMessage reply = base.ReceiveMessage(sender, type, list);

            if (sender == this)
                return ComponentReplyMessage.Empty;

            switch (type)
            {
                case ComponentMessageType.GetActorConnection:
                    reply = new ComponentReplyMessage(ComponentMessageType.ReturnActorConnection,
                                                      playerSession.ConnectedClient);
                    break;
                case ComponentMessageType.GetActorSession:
                    reply = new ComponentReplyMessage(ComponentMessageType.ReturnActorSession, playerSession);
                    break;
                case ComponentMessageType.Die:
                    playerSession.AddPostProcessingEffect(PostProcessingEffectType.Death, -1);
                    IoCManager.Resolve<IRoundManager>().CurrentGameMode.PlayerDied(playerSession);
                    // Tell the current game mode a player just died
                    break;
            }

            return reply;
        }
    }
}
