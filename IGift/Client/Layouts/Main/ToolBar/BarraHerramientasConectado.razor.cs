﻿using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasConectado
    {
        [Parameter]
        public string userName { get; set; }

        [CascadingParameter]
        private string _estiloBotones { get; set; }

        [CascadingParameter] public HubConnection _hubConnection { get; set; }


        private List<TitulosConectadoResponse> titulosConectado = new List<TitulosConectadoResponse>();
        private List<CategoriaResponse> listaCategorias = new List<CategoriaResponse>();


        private string href { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //try
            //{
            //    var response = await _titulosService.LoadConectado();
            //    if (response.Succeeded)
            //    {
            //        titulosConectado = response.Data.Titulos.ToList();
            //        listaCategorias = response.Data.Categorias.ToList();
            //    }
            //    if (string.IsNullOrEmpty(userName))
            //    {
            //        var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            //        userName = state.User.GetFirstName();
            //    }
            //    _hubConnection = await _hubConnection.TryInitialize(_nav, _localStorage);

            //    if (_hubConnection.State == HubConnectionState.Disconnected)
            //    {
            //        await _hubConnection.StartAsync();
            //    }
            //}
            //catch (Exception e)
            //{

            //    throw;
            //}

        }

        public bool _open;

        public void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }

        private string MenuStyle = "visibility:collapse";
        private void ShowMenu()
        {
            if (!string.IsNullOrEmpty(MenuStyle))
            {
                MenuStyle = string.Empty;
            }
            else
            {
                MenuStyle = "visibility:collapse";
            }
        }

        bool _expanded;
    }
}
