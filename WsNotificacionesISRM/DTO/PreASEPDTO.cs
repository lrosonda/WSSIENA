using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsNotificacionesISRM.DTO
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaEnvioPreASEP
    {
        private EncabezadoEnvioPreASEP _encabezado;
        private DetalleEnvioPreASEP[] _detalle;

        public EncabezadoEnvioPreASEP encabezado { get => _encabezado; set => _encabezado = value; }
        public DetalleEnvioPreASEP[] Detalle { get => _detalle; set => _detalle = value; }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class EncabezadoEnvioPreASEP
    {
        private String _mensajeRespuesta;
        private Int32 _codigoMensaje;
        private Int32 _totalRegistrosEnviados;
        private Int16 _numeroLote;
        public string mensajeRespuesta { get => _mensajeRespuesta; set => _mensajeRespuesta = value; }
        public int codigoMensaje { get => _codigoMensaje; set => _codigoMensaje = value; }
        public int totalRegistrosEnviados { get => _totalRegistrosEnviados; set => _totalRegistrosEnviados = value; }
        public short numeroLote { get => _numeroLote; set => _numeroLote = value; }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class DetalleEnvioPreASEP
    {
        private string _fechaSENDMSG;
        private string _MRID_SAP;
        private string _MRID_SDPNA;
        private string _MRDIDWOR;
        private string _TIPO_MSG;
        private Int32 _idNotificacionEnviada;

        public string fechaSENDMSG { get => fechaSENDMSG; set => fechaSENDMSG = value; }
        public string MRID_SAP1 { get => _MRID_SAP; set => _MRID_SAP = value; }
        public string MRID_SDPNA1 { get => _MRID_SDPNA; set => _MRID_SDPNA = value; }
        public string MRDIDWOR1 { get => _MRDIDWOR; set => _MRDIDWOR = value; }
        public string TIPO_MSG1 { get => _TIPO_MSG; set => _TIPO_MSG = value; }
        public int idNotificacionEnviada { get => _idNotificacionEnviada; set => _idNotificacionEnviada = value; }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class SolicitudConfirmacionPreASEP {
        EncabezadoConfirmacionPreASEP _encabezado;
        DetalleNotificacionFallida[] _detalle;
        public EncabezadoConfirmacionPreASEP encabezado { get => _encabezado; set => _encabezado = value; }
        public DetalleNotificacionFallida[] detalle { get => _detalle; set => _detalle = value; }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaConfirmacionPreASEP
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
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class EncabezadoConfirmacionPreASEP { 
        private Int32 _totalRecibida;
        private Int32 _totalCorrectas;
        private Int32 _totalFallidas;
        private Int32 _numeroLote;

        public int totalRecibida { get => _totalRecibida; set => _totalRecibida = value; }
        public int totalCorrectas { get => _totalCorrectas; set => _totalCorrectas = value; }
        public int totalFallidas { get => _totalFallidas; set => _totalFallidas = value; }
        public int numeroLote { get => _numeroLote; set => _numeroLote = value; }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class DetalleNotificacionFallida
    {
        private String _mensajeRespuesta;
        private Int32 _codigoMensaje;
        private Int32 _idNotificacionEnviada;
        public string mensajeRespuesta { get => _mensajeRespuesta; set => _mensajeRespuesta = value; }
        public int codigoMensaje { get => _codigoMensaje; set => _codigoMensaje = value; }
        public int idNotificacionEnviada { get => _idNotificacionEnviada; set => _idNotificacionEnviada = value; }
    }
    
}