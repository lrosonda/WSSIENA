using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsNotificacionesISRM.DTO
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class SolicitudEnvioIVR
    {
        private String _tipo;

        public string tipo { get => _tipo; set => _tipo = value; }
        public override string ToString()
        {
            return $"{this._tipo}";
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaEnvioIVR
    {
        private Int32 _identificadorMensaje;
        private String _tipo;
        private String _nombre;
        private String _segundoNombre;
        private String _apellido;
        private String _segundoApellido;
        private String _telefonoResidencial;
        private String _telefonoMovil;
        private String _areaAfectada;
        private String _estadoTrabajo;
        private String _fechaDeCorte;
        private String _fechaDeRestauracion;
        private String _mensajeRespuesta;
        private Int32 _codigoMensaje;
        public int identificadorMensaje { get => _identificadorMensaje; set => _identificadorMensaje = value; }
        public string tipo { get => _tipo; set => _tipo = value; }
        public string nombre { get => _nombre; set => _nombre = value; }
        public string segundoNombre { get => _segundoNombre; set => _segundoNombre = value; }
        public string apellido { get => _apellido; set => _apellido = value; }
        public string segundoApellido { get => _segundoApellido; set => _segundoApellido = value; }
        public string telefonoResidencial { get => _telefonoResidencial; set => _telefonoResidencial = value; }
        public string telefonoMovil { get => _telefonoMovil; set => _telefonoMovil = value; }
        public string areaAfectada { get => _areaAfectada; set => _areaAfectada = value; }
        public string estadoTrabajo { get => _estadoTrabajo; set => _estadoTrabajo = value; }
        public string fechaDeCorte { get => _fechaDeCorte; set => _fechaDeCorte = value; }
        public string fechaDeRestauracion { get => _fechaDeRestauracion; set => _fechaDeRestauracion = value; }
        public string mensajeRespuesta { get => _mensajeRespuesta; set => _mensajeRespuesta = value; }
        public int codigoMensaje { get => _codigoMensaje; set => _codigoMensaje = value; }
        public override string ToString()
        {
            return $"{this._apellido} {this._areaAfectada} {this._codigoMensaje} {this._estadoTrabajo} {this._fechaDeCorte} {this._fechaDeRestauracion} {this._identificadorMensaje} {this._mensajeRespuesta} {this._nombre} {this._segundoApellido} {this._telefonoMovil} {this._telefonoResidencial} {this._tipo}";
        }
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
        public partial class SolitudConfirmacionIVR
        {
            private Int32 _identificadorMensaje;
            private String _fechaRegistro;
            private String _estadoLLamada;
            public int identificadorMensaje { get => _identificadorMensaje; set => _identificadorMensaje = value; }
            public string fechaRegistro { get => _fechaRegistro; set => _fechaRegistro = value; }
            public string estadoLLamada { get => _estadoLLamada; set => _estadoLLamada = value; }
            public override string ToString()
            {
                return $"{this._estadoLLamada} {this._fechaRegistro} {this._identificadorMensaje}";
            }
        }
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
        public partial class RespuestaConfirmacionIVR
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
}