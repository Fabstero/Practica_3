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

    const renderInsigniaPartido = (row) => {
        const siglas = escaparHtml(row.siglas);
        if (row.banderaUrl) {
            const url = escaparHtml(row.banderaUrl);
            return `
                <div class="d-flex align-items-center gap-2">
                    <img src="${url}" alt="${siglas}" class="party-flag-thumb" onerror="this.style.display='none'" />
                    <span class="badge text-bg-primary">${siglas}</span>
                </div>`;
        }

        return `
            <div class="d-flex align-items-center gap-2">
                <span class="party-siglas-avatar">${siglas.substring(0, 4)}</span>
                <span class="badge text-bg-primary">${siglas}</span>
            </div>`;
    };

    const PartidoUI = {
        tabla: null,

        init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        inicializarTabla() {
            this.tabla = new DataTable('#tblPartidos', {
                ajax: {
                    url: '/Partido/ObtenerPartidos',
                    dataSrc: '',
                    type: 'GET'
                },
                columns: [
                    { data: 'idPartido', title: 'ID' },
                    {
                        data: 'siglas',
                        title: 'Bandera / Siglas',
                        render: (_data, _type, row) => renderInsigniaPartido(row)
                    },
                    {
                        data: 'nombre',
                        title: 'Nombre del Partido',
                        render: (data) => `<span class="fw-semibold">${escaparHtml(data)}</span>`
                    },
                    {
                        data: 'nombreCandidato',
                        title: 'Candidato',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'cantidadVotos',
                        title: 'Votos',
                        render: (data) => `<span class="badge rounded-pill text-bg-light border text-dark fw-semibold">${Number(data || 0)}</span>`
                    },
                    {
                        data: 'activo',
                        title: 'Estado',
                        render: (activo) => activo
                            ? '<span class="badge rounded-pill text-bg-success"><i class="bi bi-check-circle me-1"></i>Activo</span>'
                            : '<span class="badge rounded-pill text-bg-secondary"><i class="bi bi-pause-circle me-1"></i>Inactivo</span>'
                    },
                    {
                        data: null,
                        title: 'Acciones',
                        orderable: false,
                        searchable: false,
                        render: (_data, _type, row) => `
                            <div class="table-actions">
                                <button type="button" class="btn-action btn-action-edit btn-editar" data-id="${row.idPartido}" title="Editar partido" aria-label="Editar partido">
                                    <i class="bi bi-pencil-fill"></i>
                                </button>
                                <button type="button" class="btn-action btn-action-delete btn-eliminar" data-id="${row.idPartido}" title="Eliminar partido" aria-label="Eliminar partido">
                                    <i class="bi bi-trash3-fill"></i>
                                </button>
                            </div>`
                    }
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/es-ES.json'
                },
                pageLength: 10
            });
        },

        registrarEventos() {
            $('#formCrearPartido').on('submit', (e) => {
                e.preventDefault();
                this.crearPartido();
            });

            $('#formEditarPartido').on('submit', (e) => {
                e.preventDefault();
                this.actualizarPartido();
            });

            $('#tblPartidos').on('click', '.btn-editar', (e) => {
                const id = $(e.currentTarget).data('id');
                this.cargarPartido(id);
            });

            $('#tblPartidos').on('click', '.btn-eliminar', (e) => {
                const id = $(e.currentTarget).data('id');
                this.confirmarEliminar(id);
            });

            $('#modalCrearPartido').on('show.bs.modal', () => {
                $('#formCrearPartido')[0].reset();
                $('#crearActivo').prop('checked', true);
                this.ocultarMensaje('#mensajeCrearPartido');
            });

            $('#modalEditarPartido').on('show.bs.modal', () => {
                this.ocultarMensaje('#mensajeEditarPartido');
            });
        },

        async crearPartido() {
            const payload = {
                nombre: $('#crearNombrePartido').val().trim(),
                siglas: $('#crearSiglas').val().trim().toUpperCase(),
                banderaUrl: $('#crearBanderaUrl').val().trim() || null,
                nombreCandidato: $('#crearNombreCandidato').val().trim(),
                activo: $('#crearActivo').is(':checked')
            };

            if (!payload.nombre || !payload.siglas || !payload.nombreCandidato) {
                this.mostrarMensaje('#mensajeCrearPartido', 'Nombre, siglas y candidato son obligatorios.', 'danger');
                return;
            }

            try {
                const res = await fetch('/Partido/CrearPartido', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();
                if (!res.ok || !data.esCorrecto) {
                    this.mostrarMensaje('#mensajeCrearPartido', data.mensaje || 'No se pudo registrar el partido.', 'danger');
                    return;
                }

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalCrearPartido')).hide();
                this.tabla.ajax.reload(null, false);
                Swal.fire({
                    icon: 'success',
                    title: 'Partido registrado',
                    text: data.mensaje,
                    timer: 1800,
                    showConfirmButton: false
                });
            } catch {
                this.mostrarMensaje('#mensajeCrearPartido', 'Error de comunicación con el servidor.', 'danger');
            }
        },

        async cargarPartido(id) {
            try {
                const res = await fetch(`/Partido/ObtenerPartidoPorId/${id}`);
                if (!res.ok) {
                    throw new Error();
                }

                const partido = await res.json();
                $('#editarIdPartido').val(partido.idPartido);
                $('#editarNombrePartido').val(partido.nombre);
                $('#editarSiglas').val(partido.siglas);
                $('#editarBanderaUrl').val(partido.banderaUrl || '');
                $('#editarNombreCandidato').val(partido.nombreCandidato);
                $('#editarActivo').prop('checked', Boolean(partido.activo));

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEditarPartido')).show();
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo cargar la información del partido político.'
                });
            }
        },

        async actualizarPartido() {
            const payload = {
                idPartido: parseInt($('#editarIdPartido').val(), 10),
                nombre: $('#editarNombrePartido').val().trim(),
                siglas: $('#editarSiglas').val().trim().toUpperCase(),
                banderaUrl: $('#editarBanderaUrl').val().trim() || null,
                nombreCandidato: $('#editarNombreCandidato').val().trim(),
                activo: $('#editarActivo').is(':checked')
            };

            if (!payload.nombre || !payload.siglas || !payload.nombreCandidato) {
                this.mostrarMensaje('#mensajeEditarPartido', 'Nombre, siglas y candidato son obligatorios.', 'danger');
                return;
            }

            try {
                const res = await fetch('/Partido/ActualizarPartido', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();
                if (!res.ok || !data.esCorrecto) {
                    this.mostrarMensaje('#mensajeEditarPartido', data.mensaje || 'No se pudo actualizar el partido.', 'danger');
                    return;
                }

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEditarPartido')).hide();
                this.tabla.ajax.reload(null, false);
                Swal.fire({
                    icon: 'success',
                    title: 'Partido actualizado',
                    text: data.mensaje,
                    timer: 1800,
                    showConfirmButton: false
                });
            } catch {
                this.mostrarMensaje('#mensajeEditarPartido', 'Error de comunicación con el servidor.', 'danger');
            }
        },

        confirmarEliminar(id) {
            Swal.fire({
                title: '¿Eliminar partido político?',
                text: 'Si el partido ya recibió votos, la eliminación será bloqueada (ON DELETE RESTRICT).',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.eliminarPartido(id);
                }
            });
        },

        async eliminarPartido(id) {
            try {
                const res = await fetch(`/Partido/EliminarPartido/${id}`, {
                    method: 'DELETE'
                });
                const data = await res.json();

                if (!res.ok || !data.esCorrecto) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Operación bloqueada',
                        text: data.mensaje || 'No se pudo eliminar el partido político.'
                    });
                    return;
                }

                this.tabla.ajax.reload(null, false);
                Swal.fire({
                    icon: 'success',
                    title: 'Eliminado',
                    text: data.mensaje,
                    timer: 1800,
                    showConfirmButton: false
                });
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Ocurrió un error al intentar eliminar el partido político.'
                });
            }
        },

        mostrarMensaje(selector, mensaje, tipo) {
            $(selector)
                .removeClass('d-none alert-danger alert-success alert-warning')
                .addClass(`alert-${tipo}`)
                .text(mensaje);
        },

        ocultarMensaje(selector) {
            $(selector).addClass('d-none').removeClass('alert-danger alert-success alert-warning').text('');
        }
    };

    $(document).ready(() => PartidoUI.init());
})();
