using System;
using System.Net;
using System.Collections.Generic;
using SteamKit2;
using SteamTrade;
using SteamTrade.TradeOffer;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

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
                Log.Warn("El usuario con ID: " + OtherSID + " ,me mandó una petición de amistad, mi respuesta fue: {0}", false);
                return false;
            }
        }

        /// <summary>
        /// Es llamado cuando un usuario borra al bot como amigo.
        /// </summary>
        public override void OnFriendRemove() {
            Log.Info("El usuario: " + OtherSID + " ,me borró como amigo.");
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
            ///Leer linea 531 de la clase Bot.cs
        }

        /// <summary>
        /// Es llamado cuando el bot recibe un mensaje de chat.
        /// </summary>
        public override void OnMessage(string message, EChatEntryType type) {

            if(message == "Hola!") {
                SendChatMessage("Hola! Soy Hermes, un bot diseñado por Vossile.");
            }
            if(message.StartsWith("!admin.")){
                ///Nos quedamos con la ID del arma
                string idmini = message.Substring(7);
                //Comprobamos que tenga los valores normales de una skin de CSGO
                if(EsIDSkin(idmini)) {
                    SendChatMessage("Se obtendrá su inventario y a continuacion el nombre de la skin con ID: {0} .",idmini);

                    ///Convertimos la ID de la otra persona en steamid64
                    ulong _idObjetivo = OtherSID.ConvertToUInt64();
                    sacarInventario(_idObjetivo);
                    SendChatMessage("Inventario obtenido.");
                }
                
            } else {
                SendChatMessage(Bot.ChatResponse);
            }
        }

        /// <summary>
        /// Obtiene el inventario de la ID pasada
        /// </summary>
        /// <param name="other">SteamId64 del usuario.</param>
        public void sacarInventario (ulong id) {

            /// Incluimos en 'url' la URL completa junto al ID del usuario
            string options = string.Format("{0}", id);
            string url = String.Format(Constantes.BASEURL, options);

            /// Hacemos el GET a la pagina
            var request = HttpWebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            string texto;
            var respuesta = (HttpWebResponse)request.GetResponse();

            ///Guardamos el resultado en un string con formato JSON
            using(var sr = new StreamReader(respuesta.GetResponseStream())) {
                texto = sr.ReadToEnd();
            }

            JObject busqueda = JObject.Parse(texto);
            IList<JToken> results = busqueda.Children().ToList();

            IList<CSGOskinsIdentifier> identificadores = new List<CSGOskinsIdentifier>();
            foreach(JToken res in results) {
                CSGOskinsIdentifier identificador = JsonConvert.DeserializeObject<CSGOskinsIdentifier>(res.ToString());
                identificadores.Add(identificador);
            }

            Log.Info(identificadores.Count.ToString());

        }

        /// <summary>
        /// Crea un nuevo intercambio con el usuario pasado
        /// </summary>
        /// <param name="other">SteamId del usuario objetivo.</param>
        public TradeOffer NewTradeOffer(SteamID other) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///Es llamado cuando el usuario acepta el intercambio.
        /// </summary>
        public override void OnTradeAccept() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Es llamado cuando el bot recibe una peticion de intercambio.
        /// </summary>
        public override bool OnTradeRequest() {
            return true;
        }

        /// <summary>
        /// Es llamado cuando elusuario acepta/rechaza la peticion de intercambio.
        /// </summary>
        /// <param name="accepted">True si acepto, false si no.</param>
        /// <param name="response">Respuesta en String a la llamada.</param>
        public override void OnTradeRequestReply(bool accepted, string response) {
            if(!accepted) SendChatMessage("Has rechazado el intercambio.");
            else SendChatMessage("Intercambio realizado con exito.");
        }

        public override void OnTradeError(string error) {
            throw new NotImplementedException();
        }

        public override void OnTradeTimeout() {
            throw new NotImplementedException();
        }

        public override void OnTradeSuccess() {
            throw new NotImplementedException();
        }

        public override void OnTradeAwaitingConfirmation(long tradeOfferID) {
            throw new NotImplementedException();
        }

        public override void OnTradeInit() {
            throw new NotImplementedException();
        }

        public override void OnTradeAddItem(Schema.Item schemaItem, Inventory.Item inventoryItem) {
            throw new NotImplementedException();
        }

        public override void OnTradeRemoveItem(Schema.Item schemaItem, Inventory.Item inventoryItem) {
            throw new NotImplementedException();
        }

        public override void OnTradeMessage(string message) {
            throw new NotImplementedException();
        }

        public override void OnTradeReady(bool ready) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cuando un usuario pide cambiar skins suyas por puntos en la web.
        /// </summary>
        public void VendiendoSkins(SteamID usuario, List<Schema.Item> listaDeArmas) {
            for(int i = 0; i < 0; i++) {
                //   OnTraddeAddItem (listaDeArmas[i],);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Comprueba si el objeto que se le pasa es una ID de una skin.
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns>True si es una ID valida</returns>
        public bool EsIDSkin(object Expression) {
            bool isNum = false;
            bool tam9 = false;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            tam9 = Convert.ToString(Expression).Length == 9;

            if(isNum == true) {
                if(tam9 == true) {
                    return true;
                } else {
                    SendChatMessage("No tiene 9 digitos");
                    return false;
                }
            } else {
                SendChatMessage("No es un numero");
                return false;
            } 
        }
    }
}
