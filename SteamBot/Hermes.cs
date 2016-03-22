using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamTrade;

namespace SteamBot
{
    public class Hermes : UserHandler
    { 
        public Hermes(Bot bot, SteamID sid) : base(bot, sid) { }

        /// <summary>
        /// Es llamado cuando un usuario manda una peticion de amistad al bot.
        /// </summary>
        /// <returns>
        /// True si es administrador y False si no lo es.
        /// </returns>
        public override bool OnFriendAdd() {
            if (IsAdmin) {
                Log.Info("El admin con ID: " + OtherSID + " ,me mandó una petición de amistad, mi respuesta fue: {0}", true,".");
                return true;
            }else{
                if (OtherSID==76561198094066023) return true;

                Log.Warn("El usuario con ID: " + OtherSID + " ,me mandó una petición de amistad, mi respuesta fue: {0}", false);
                return false;
            }
        }

        /// <summary>
        /// Es llamado cuando un usuario borra al bot como amigo.
        /// </summary>
        public override void OnFriendRemove() {
            Log.Info("El usuario: " + OtherSID + " ,me borro como amigo.");
        }

        /// <summary>
        /// Es llamado cuando invitan al bot a un grupo de Steam.
        /// </summary>
        /// <returns>
        /// Se rechaza por defecto.
        /// </returns>
        public override bool OnGroupAdd() { return false; }

        /// <summary>
        /// Es llamado cuando el bot se ha logeado correctamente.
        /// </summary>
        public override void OnLoginCompleted(){
            ///Leer linea 532 de la clase Bot.cs
        }

        /// <summary>
        /// Es llamado cuando el bot recibe un mensaje de chat.
        /// </summary>
        public override void OnMessage(string message, EChatEntryType type)
        {
            if(message=="Quien es tu creador?") SendChatMessage("¿El mio? Belcebú y ¿el tuyo?");
            if (message == "Hola!") SendChatMessage("Hola! Soy Hermes, un bot diseñado por Vossile.");

            else SendChatMessage(Bot.ChatResponse);
        }

        /// <summary>
        ///Es llamado cuando el usuario acepta el trade.
        /// </summary>
        public override void OnTradeAccept()
        {
            throw new NotImplementedException();
        }

        public override void OnTradeAddItem(Schema.Item schemaItem, Inventory.Item inventoryItem)
        {
            throw new NotImplementedException();
        }

        public override void OnTradeAwaitingConfirmation(long tradeOfferID)
        {
            throw new NotImplementedException();
        }

        public override void OnTradeError(string error)
        {
            throw new NotImplementedException();
        }

        public override void OnTradeInit()
        {
            throw new NotImplementedException();
        }

        public override void OnTradeMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnTradeReady(bool ready)
        {
            throw new NotImplementedException();
        }

        public override void OnTradeRemoveItem(Schema.Item schemaItem, Inventory.Item inventoryItem)
        {
            throw new NotImplementedException();
        }

        public override bool OnTradeRequest()
        {
            throw new NotImplementedException();
        }

        public override void OnTradeSuccess()
        {
            throw new NotImplementedException();
        }

        public override void OnTradeTimeout()
        {
            throw new NotImplementedException();
        }
    }
}
