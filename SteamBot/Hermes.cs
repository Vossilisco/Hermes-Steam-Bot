using System;
using System.Net;
using System.Collections.Generic;
using SteamKit2;
using SteamTrade;
using SteamTrade.TradeOffer;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

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

            ///Para revisar si el perfil de alguien es publico o privado
            if(message.StartsWith("!admin.checkinvent.")) {

                string minid = message.Split('.')[2];
                if(minid.Length == 17 && EsNumero(minid)) {

                    SendChatMessage("Se intentará el revisar el inventario del usuario con ID64: {0}", minid);

                    ///Convertimos la ID de la otra persona en steamid64
                    ulong idObjetivo = Convert.ToUInt64(minid);

                    ///Llamamos a la funcion para ver si su inventario es publico o privado
                    if(InventarioPublico(idObjetivo)) {
                        SendChatMessage("Se puede obtener su inventario, es publico.");
                    } else {
                        SendChatMessage("No se puede obtener su inventario, es privado.");
                    }
                } else {
                    SendChatMessage("ID64 no reconocida: {0} .", minid);
                }

            } else {
                ///Para saber si alguien tiene un arma en concreto, sabiendo el id de esta.
                if(message.StartsWith("!admin.checkweapon.")) {
                    string minid;
                    string armaid;
                    string nombreDelArma = "";
                    ulong idObjetivo;

                    minid = message.Split('.')[2];
                    armaid = message.Split('.')[3];

                    idObjetivo = Convert.ToUInt64(minid);
                    ///HACE FALTA METER COMPROBACIONES DE MINID Y ARMAID

                    nombreDelArma = NombreArma(idObjetivo, armaid);
                    if(InventarioPublico(idObjetivo)) {
                        SendChatMessage("El ID: {0} pertenece a: {1}  .", armaid, nombreDelArma);
                    } else {
                        SendChatMessage("Error, su inventario es privado.");
                    }
                } else {
                    ///Te dice tu ID y tu ID64.
                    if(message == "!admin.myid") {

                        SendChatMessage("Tu ID es: {0}  y tu ID64 es: {1}  .",OtherSID,OtherSID.ConvertToUInt64());
                    } else {
                        //Para todo lo demas... "Buenos dias!"
                        SendChatMessage(Bot.ChatResponse);
                    }
                }
            }
        }

        /// <summary>
        /// Intenta obtener el inventario de la ID pasada.
        /// </summary>
        /// <param name="accepted">ID 64 del usuario propietario del inventario.</param>
        /// <param name="response">True si es publico, false si no.</param>
        bool InventarioPublico (ulong id) {
            /// En este string pondremos el inventario del id en formato json.
            string inventario;
            
            /// Incluimos en 'url' la URL completa junto al ID del usuario.
            string options = string.Format("{0}", id);
            string url = String.Format(Constantes.BASEURL, options);

            /// Hacemos el GET a la pagina para obtener el inventario en json.
            var requeste = WebRequest.Create(url);
            HttpWebResponse webResponse = ( HttpWebResponse )requeste.GetResponse();
            StreamReader fichero = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            inventario = fichero.ReadToEnd();

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(inventario);

            ///Comprobamos el campo success para saber si se ha podido coger el inventario
            if(dict["success"].ToString() == "True") { 
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el nombre de un arma del inventario de la ID pasada.
        /// </summary>
        /// <param name="accepted">ID 64 del usuario propietario del inventario.</param>
        /// <param name="response">String del nombre del arma.</param>
        string NombreArma(ulong id,string armaid) {
            /// En este string pondremos el inventario del id en formato json.

            string inventario;
            string classidDESCR = "";

            /// Incluimos en 'url' la URL completa junto al ID del usuario.
            string options = string.Format("{0}", id);
            string url = String.Format(Constantes.BASEURL, options);

            /// Hacemos el GET a la pagina para obtener el inventario en json.
            var requeste = WebRequest.Create(url);
            HttpWebResponse webResponse = ( HttpWebResponse )requeste.GetResponse();
            StreamReader fichero = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            inventario = fichero.ReadToEnd();

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(inventario);

            ///Recorremos el JSON, concretamente las Descripciones.
            foreach(var descript in dict["rgDescriptions"]) {
                classidDESCR = descript.Key.ToString();
                /// Hasta que hallamos una que tenga el mismo ID que la que entra y devolvemos su atributo name.
                if(armaid == classidDESCR.Split('_')[0]) {
                    string name = dict["rgDescriptions"][classidDESCR]["name"].ToString();
                    return name;
                }
            }
            return "Arma no encontrada";
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
        /// Comprueba si el objeto que se le pasa es un numero;
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns>True si es un numero</returns>
        public bool EsNumero(object Expression) {
            bool isNum = false;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }
    }
}
