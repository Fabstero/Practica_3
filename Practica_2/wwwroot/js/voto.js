(() => {
    const escaparHtml = (valor) => {
        if (valor === null || valor === undefined) return '';
        return String(valor)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    };

    const VotoUI = {
        tabla: null,
        votanteSeleccionado: null,
        partidoSeleccionadoId: null,
        partidosActivos: [],

        init() {
            this.inicializarTabla();
            this.registrarEventos();
            this.cargarTodo();
        },

        inicializarTabla() {
            this.tabla = new DataTable('#tblVotos', {
                ajax: {
                    url: '/Voto/ObtenerVotos',
                    dataSrc: '',
                    type: 'GET'
                },
                order: [[0, 'desc']],
                columns: [
                    { data: 'idVoto', title: 'ID Voto' },
                    {
                        data: 'cedulaVotante',
                        title: 'Cédula',
                        render: (data) => `<span class="font-monospace fw-semibold">${escaparHtml(data)}</span>`
                    },
                    {
                        data: 'nombreVotante',
                        title: 'Votante',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'provinciaVotante',
                        title: 'Provincia',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: null,
                        title: 'Partido',
                        render: (_data, _type, row) => `
                            <span class="badge text-bg-primary me-1">${escaparHtml(row.siglasPartido)}</span>
                            <span>${escaparHtml(row.nombrePartido)}</span>`
                    },
                    {
                        data: 'nombreCandidato',
                        title: 'Candidato',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'fechaHoraVoto',
                        title: 'Fecha y Hora',
                        render: (data) => `<span class="text-muted small">${escaparHtml(data)}</span>`
                    }
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/es-ES.json'
                },
                pageLength: 10
            });
        },

        registrarEventos() {
            $('#btnBuscarCedula').on('click', () => this.buscarPorCedula());
            $('#inputBuscarCedula').on('keydown', (e) => {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    this.buscarPorCedula();
                }
            });

            $('#selectVotantePendiente').on('change', (e) => {
                const opcion = e.target.selectedOptions[0];
                if (!opcion || !opcion.value) {
                    this.limpiarVotanteSeleccionado();
                    return;
                }

                const votante = JSON.parse(opcion.dataset.votante);
                $('#inputBuscarCedula').val(votante.cedula);
                this.establecerVotanteSeleccionado(votante);
            });

            $('#listaPartidosUrna').on('click', '.party-ballot-card', (e) => {
                const id = parseInt($(e.currentTarget).data('id'), 10);
                this.seleccionarPartido(id);
            });

            $('#btnEmitirVoto').on('click', () => this.confirmarVoto());
            $('#btnRefrescarResultados').on('click', () => this.cargarTodo());
        },

        async cargarTodo() {
            await Promise.all([
                this.cargarVotantesPendientes(),
                this.cargarPartidosActivos(),
                this.cargarResumen()
            ]);
            if (this.tabla) {
                this.tabla.ajax.reload(null, false);
            }
        },

        async cargarVotantesPendientes() {
            try {
                const res = await fetch('/Votante/ObtenerPendientes');
                const pendientes = await res.json();
                const select = $('#selectVotantePendiente');
                select.empty();

                if (!pendientes.length) {
                    select.append('<option value="">No hay votantes pendientes</option>');
                    return;
                }

                select.append('<option value="">-- Seleccionar votante pendiente --</option>');
                pendientes.forEach((v) => {
                    const opt = document.createElement('option');
                    opt.value = v.idVotante;
                    opt.textContent = `${v.cedula} - ${v.nombreCompleto} (${v.provincia})`;
                    opt.dataset.votante = JSON.stringify(v);
                    select.append(opt);
                });
            } catch {
                // Silenciar fallo transitorio
            }
        },

        async cargarPartidosActivos() {
            try {
                const res = await fetch('/Partido/ObtenerPartidosActivos');
                this.partidosActivos = await res.json();
                this.renderizarPartidosUrna();
            } catch {
                $('#listaPartidosUrna').html('<div class="col-12 text-danger">No se pudieron cargar los partidos activos.</div>');
            }
        },

        renderizarPartidosUrna() {
            const contenedor = $('#listaPartidosUrna');
            contenedor.empty();

            if (!this.partidosActivos.length) {
                contenedor.html('<div class="col-12"><div class="alert alert-warning mb-0">No hay partidos políticos activos disponibles para votar.</div></div>');
                return;
            }

            this.partidosActivos.forEach((p) => {
                const seleccionado = this.partidoSeleccionadoId === p.idPartido ? 'selected' : '';
                const insignia = p.banderaUrl
                    ? `<img src="${escaparHtml(p.banderaUrl)}" alt="${escaparHtml(p.siglas)}" class="party-flag-ballot" onerror="this.style.display='none'" />`
                    : `<div class="party-siglas-avatar party-siglas-avatar-lg">${escaparHtml(p.siglas.substring(0, 4))}</div>`;

                contenedor.append(`
                    <div class="col-md-6">
                        <div class="party-ballot-card ${seleccionado}" data-id="${p.idPartido}" role="button" tabindex="0">
                            <div class="d-flex align-items-center gap-3">
                                ${insignia}
                                <div class="flex-grow-1">
                                    <div class="d-flex align-items-center justify-content-between gap-2">
                                        <span class="badge text-bg-primary">${escaparHtml(p.siglas)}</span>
                                        <i class="bi bi-check-circle-fill ballot-check-icon"></i>
                                    </div>
                                    <h6 class="mb-0 mt-1 fw-bold">${escaparHtml(p.nombre)}</h6>
                                    <small class="text-muted"><i class="bi bi-person me-1"></i>${escaparHtml(p.nombreCandidato)}</small>
                                </div>
                            </div>
                        </div>
                    </div>`);
            });
        },

        async cargarResumen() {
            try {
                const res = await fetch('/Voto/ObtenerResumen');
                const resumen = await res.json();

                $('#metricTotalVotantes').text(resumen.totalVotantes);
                $('#metricTotalVotos').text(resumen.totalVotosEmitidos);
                $('#metricPendientes').text(resumen.votantesPendientes);
                $('#metricParticipacion').text(`${Number(resumen.porcentajeParticipacion || 0).toFixed(1)}%`);

                const contenedor = $('#contenedorEscrutinio');
                contenedor.empty();

                if (!resumen.resultadosPorPartido || !resumen.resultadosPorPartido.length) {
                    contenedor.html('<p class="text-muted mb-0">No hay partidos políticos registrados.</p>');
                    return;
                }

                resumen.resultadosPorPartido.forEach((item, index) => {
                    const colorClase = index === 0 && item.totalVotos > 0 ? 'bg-success' : 'bg-primary';
                    const badgeEstado = item.activo
                        ? ''
                        : '<span class="badge text-bg-secondary ms-1">Inactivo</span>';

                    contenedor.append(`
                        <div class="scrutiny-item mb-3">
                            <div class="d-flex justify-content-between align-items-baseline mb-1">
                                <div>
                                    <span class="badge text-bg-primary me-1">${escaparHtml(item.siglas)}</span>
                                    <span class="fw-semibold">${escaparHtml(item.nombre)}</span>
                                    ${badgeEstado}
                                    <div class="small text-muted">${escaparHtml(item.nombreCandidato)}</div>
                                </div>
                                <div class="text-end text-nowrap ms-2">
                                    <span class="fw-bold">${item.totalVotos}</span>
                                    <small class="text-muted">(${Number(item.porcentaje || 0).toFixed(1)}%)</small>
                                </div>
                            </div>
                            <div class="progress scrutiny-progress" role="progressbar" aria-valuenow="${item.porcentaje}" aria-valuemin="0" aria-valuemax="100">
                                <div class="progress-bar ${colorClase}" style="width: ${item.porcentaje}%"></div>
                            </div>
                        </div>`);
                });
            } catch {
                $('#contenedorEscrutinio').html('<p class="text-danger mb-0">Error al cargar el escrutinio.</p>');
            }
        },

        async buscarPorCedula() {
            const cedula = $('#inputBuscarCedula').val().trim();
            this.ocultarMensaje();

            if (!cedula) {
                this.mostrarMensaje('Ingrese una cédula para buscar en el padrón.', 'warning');
                return;
            }

            try {
                const res = await fetch(`/Votante/BuscarPorCedula?cedula=${encodeURIComponent(cedula)}`);
                if (!res.ok) {
                    const err = await res.json().catch(() => ({}));
                    this.limpiarVotanteSeleccionado();
                    this.mostrarMensaje(err.mensaje || 'No se encontró un votante con esa cédula.', 'danger');
                    return;
                }

                const votante = await res.json();
                this.establecerVotanteSeleccionado(votante);
                $('#selectVotantePendiente').val(votante.haVotado ? '' : String(votante.idVotante));
            } catch {
                this.mostrarMensaje('Error al consultar el padrón electoral.', 'danger');
            }
        },

        establecerVotanteSeleccionado(votante) {
            this.votanteSeleccionado = votante;
            this.ocultarMensaje();

            $('#votanteSelNombre').text(votante.nombreCompleto);
            $('#votanteSelCedula').text(votante.cedula);
            $('#votanteSelProvincia').text(votante.provincia);

            const badge = $('#votanteSelEstado');
            if (votante.haVotado) {
                badge.attr('class', 'badge rounded-pill text-bg-danger').text('Ya emitió su voto');
                this.mostrarMensaje('Este votante ya emitió su voto anteriormente y no puede volver a votar.', 'warning');
            } else {
                badge.attr('class', 'badge rounded-pill text-bg-success').text('Habilitado para votar');
            }

            $('#tarjetaVotanteSeleccionado').removeClass('d-none');
            this.actualizarEstadoBotonVotar();
        },

        limpiarVotanteSeleccionado() {
            this.votanteSeleccionado = null;
            $('#tarjetaVotanteSeleccionado').addClass('d-none');
            this.actualizarEstadoBotonVotar();
        },

        seleccionarPartido(idPartido) {
            this.partidoSeleccionadoId = idPartido;
            $('.party-ballot-card').removeClass('selected');
            $(`.party-ballot-card[data-id="${idPartido}"]`).addClass('selected');
            this.actualizarEstadoBotonVotar();
        },

        actualizarEstadoBotonVotar() {
            const habilitado = Boolean(
                this.votanteSeleccionado &&
                !this.votanteSeleccionado.haVotado &&
                this.partidoSeleccionadoId
            );
            $('#btnEmitirVoto').prop('disabled', !habilitado);
        },

        confirmarVoto() {
            if (!this.votanteSeleccionado || !this.partidoSeleccionadoId) {
                return;
            }

            const partido = this.partidosActivos.find((p) => p.idPartido === this.partidoSeleccionadoId);
            Swal.fire({
                title: 'Confirmar emisión de voto',
                html: `<strong>Votante:</strong> ${escaparHtml(this.votanteSeleccionado.nombreCompleto)} (${escaparHtml(this.votanteSeleccionado.cedula)})<br/>
                       <strong>Partido:</strong> ${escaparHtml(partido?.nombre || '')} (${escaparHtml(partido?.siglas || '')})`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#198754',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Emitir voto',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.emitirVoto();
                }
            });
        },

        async emitirVoto() {
            try {
                const res = await fetch('/Voto/EmitirVoto', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        idVotante: this.votanteSeleccionado.idVotante,
                        cedula: this.votanteSeleccionado.cedula,
                        idPartido: this.partidoSeleccionadoId
                    })
                });

                const data = await res.json();
                if (!res.ok || !data.esCorrecto) {
                    Swal.fire({
                        icon: 'error',
                        title: 'No se pudo registrar el voto',
                        text: data.mensaje || 'Error al emitir el voto.'
                    });
                    return;
                }

                this.partidoSeleccionadoId = null;
                this.limpiarVotanteSeleccionado();
                $('#inputBuscarCedula').val('');
                $('#selectVotantePendiente').val('');

                await this.cargarTodo();

                Swal.fire({
                    icon: 'success',
                    title: '¡Voto registrado!',
                    text: data.mensaje,
                    timer: 2200,
                    showConfirmButton: false
                });
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Ocurrió un error de comunicación al registrar el voto.'
                });
            }
        },

        mostrarMensaje(texto, tipo) {
            $('#mensajeUrna')
                .removeClass('d-none alert-danger alert-warning alert-success')
                .addClass(`alert-${tipo}`)
                .text(texto);
        },

        ocultarMensaje() {
            $('#mensajeUrna').addClass('d-none').removeClass('alert-danger alert-warning alert-success').text('');
        }
    };

    $(document).ready(() => VotoUI.init());
})();
