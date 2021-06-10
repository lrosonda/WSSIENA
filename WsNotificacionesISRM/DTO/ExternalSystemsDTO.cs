using System;

namespace WsNotificacionesISRM.DTO
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class SolicitudSistemaExternos
    {
        private string _correoOrigen;
        private string _correosDestinatarios;
        private string _correosCC;
        private string _correosCO;
        private string _asunto;
        private string _mensaje;
        private string _codigoSiExt;
        private string _descripcionSiExt;
        private string _path;
        private Int32 _cantidadReenvio;

        public string correoOrigen { get => _correoOrigen; set => _correoOrigen = value; }
        public string correosDestinatarios { get => _correosDestinatarios; set => _correosDestinatarios = value; }
        public string correosCC { get => _correosCC; set => _correosCC = value; }
        public string correosCO { get => _correosCO; set => _correosCO = value; }
        public string asunto { get => _asunto; set => _asunto = value; }
        public string mensaje { get => _mensaje; set => _mensaje = value; }
        public string codigoSiExt { get => _codigoSiExt; set => _codigoSiExt = value; }
        public string descripcionSiExt { get => _descripcionSiExt; set => _descripcionSiExt = value; }
        public string path { get => _path; set => _path = value; }
        public int cantidadReenvio { get => _cantidadReenvio; set => _cantidadReenvio = value; }
        public override string ToString()
        {
            return $"{this._asunto}{this._cantidadReenvio}{this._codigoSiExt}{this._correoOrigen}{this._correosCC}{this._correosCO}{this._correosDestinatarios}{this._descripcionSiExt}{this._mensaje}{this._path}";
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaSistemaExternos
    {
        private String _mensajeRespuesta;
        private Int32 _codigoMensaje;
        public string mensajeRespuesta { get => _mensajeRespuesta; set => _mensajeRespuesta = value; }
        public int codigoMensaje { get => _codigoMensaje; set => _codigoMensaje = value; }
        public override string ToString()
        {
            return $"{this._codigoMensaje} {this._mensajeRespuesta}";
        }
    }
}