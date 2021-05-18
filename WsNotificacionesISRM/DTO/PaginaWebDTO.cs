﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsNotificacionesISRM.DTO
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaNotificaciones
    {
        private Encabezado _encabezado;
        private Detalle[] _detalle;

        public Encabezado encabezado { get => _encabezado; set => _encabezado = value; }
        public Detalle[] detalle { get => _detalle; set => _detalle = value; }
        public override string ToString() {
            return $"{this._encabezado}{this._detalle}";
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class Encabezado {
        private String _mensajeRespuesta;
        private Int32 _codigoMensaje;
        private Int32 _totalEnviada;
        public string mensajeRespuesta { get => _mensajeRespuesta; set => _mensajeRespuesta = value; }
        public int codigoMensaje { get => _codigoMensaje; set => _codigoMensaje = value; }
        public int totalEnviada { get => _totalEnviada; set => _totalEnviada = value; }
        public override string ToString()
        {
            return $"{this._codigoMensaje}{this._mensajeRespuesta}{this._totalEnviada}"; 
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class Detalle { 
        private String _areaAfectada;
        private String _fechaAfetacion;
        private Int32 _horaInicioAfectacion;
        private Int32 _horaFinAfectacion;
        private Int32 _cantidadClienteAfectado;
        private String _horasAfectacion;
        private String _duracion;

        public string areaAfectada { get => _areaAfectada; set => _areaAfectada = value; }
        public string fechaAfetacion { get => _fechaAfetacion; set => _fechaAfetacion = value; }
        public int horaInicioAfectacion { get => _horaInicioAfectacion; set => _horaInicioAfectacion = value; }
        public int horaFinAfectacion { get => _horaFinAfectacion; set => _horaFinAfectacion = value; }
        public int cantidadClienteAfectado { get => _cantidadClienteAfectado; set => _cantidadClienteAfectado = value; }
        public string corasAfectacion { get => _horasAfectacion; set => _horasAfectacion = value; }
        public string duracion { get => _duracion; set => _duracion = value; }

        public override string ToString()
        {
            return $"{this._cantidadClienteAfectado}{this._areaAfectada}{this._duracion}{this._fechaAfetacion}{this._horaFinAfectacion}{this._horaInicioAfectacion}"; 
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class SolicitudCorteAREA {
        private String _areaAfectada;

        public string areaAfectada { get => _areaAfectada; set => _areaAfectada = value; }
        public override string ToString()
        {
            return $"{this._areaAfectada}";
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class SolicitudCorteNAC
    {
        private String _NAC;

        public string NAC { get => _NAC; set => _NAC = value; }
        public override string ToString()
        {
            return $"{this._NAC}";
        }
    }
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class DetalleNAC
    {
        private String _areaAfectada;
        private String _fechaAfetacion;
        private Int32 _horaInicioAfectacion;
        private Int32 _horaFinAfectacion;
        private String _horasAfectacion;
        private String _duracion;

        public string areaAfectada { get => _areaAfectada; set => _areaAfectada = value; }
        public string fechaAfetacion { get => _fechaAfetacion; set => _fechaAfetacion = value; }
        public int horaInicioAfectacion { get => _horaInicioAfectacion; set => _horaInicioAfectacion = value; }
        public int horaFinAfectacion { get => _horaFinAfectacion; set => _horaFinAfectacion = value; }
        public string horasAfectacion { get => _horasAfectacion; set => _horasAfectacion = value; }
        public string duracion { get => _duracion; set => _duracion = value; }

        public override string ToString()
        {
            return $"{this._areaAfectada}{this._duracion}{this._fechaAfetacion}{this._horaFinAfectacion}{this._horaInicioAfectacion}{this._horaInicioAfectacion}{this._horasAfectacion}";
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ensa.com.pa/")]
    public partial class RespuestaCorteNAC {
        private DetalleNAC[] _detalle;
        private Encabezado _encabezado;

        public DetalleNAC[] detalle { get => _detalle; set => _detalle = value; }
        public Encabezado encabezado { get => _encabezado; set => _encabezado = value; }
        public override string ToString()
        {
            return $"{this._detalle}{this._encabezado}";
        }
    }

}