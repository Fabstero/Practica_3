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

    const VotanteUI = {
        tabla: null,

        init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        inicializarTabla() {
            this.tabla = new DataTable('#tblVotantes', {
                ajax: {
                    url: '/Votante/ObtenerVotantes',
                    dataSrc: '',
                    type: 'GET'
                },
                columns: [
                    { data: 'idVotante', title: 'ID' },
                    {
                        data: 'cedula',
                        title: 'Cédula',
                        render: (data) => `<span class="fw-semibold font-monospace">${escaparHtml(data)}</span>`
                    },
                    {
                        data: 'nombreCompleto',
                        title: 'Nombre Completo',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'fechaNacimiento',
                        title: 'Fecha Nacimiento',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'provincia',
                        title: 'Provincia',
                        render: (data) => escaparHtml(data)
                    },
                    {
                        data: 'haVotado',
                        title: 'Estado',
                        render: (haVotado) => haVotado
                            ? '<span class="badge rounded-pill text-bg-success"><i class="bi bi-check-circle me-1"></i>Ya votó</span>'
                            : '<span class="badge rounded-pill text-bg-secondary"><i class="bi bi-clock me-1"></i>Pendiente</span>'
                    },
                    {
                        data: null,
                        title: 'Acciones',
                        orderable: false,
                        searchable: false,
                        render: (_data, _type, row) => `
                            <div class="table-actions">
                                <button type="button" class="btn-action btn-action-edit btn-editar" data-id="${row.idVotante}" title="Editar votante" aria-label="Editar votante">
                                    <i class="bi bi-pencil-fill"></i>
                                </button>
                                <button type="button" class="btn-action btn-action-delete btn-eliminar" data-id="${row.idVotante}" title="Eliminar votante" aria-label="Eliminar votante">
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
            $('#formCrearVotante').on('submit', (e) => {
                e.preventDefault();
                this.crearVotante();
            });

            $('#formEditarVotante').on('submit', (e) => {
                e.preventDefault();
                this.actualizarVotante();
            });

            $('#tblVotantes').on('click', '.btn-editar', (e) => {
                const id = $(e.currentTarget).data('id');
                this.cargarVotante(id);
            });

            $('#tblVotantes').on('click', '.btn-eliminar', (e) => {
                const id = $(e.currentTarget).data('id');
                this.confirmarEliminar(id);
            });

            $('#modalCrearVotante').on('show.bs.modal', () => {
                $('#formCrearVotante')[0].reset();
                this.ocultarMensaje('#mensajeCrearVotante');
            });

            $('#modalEditarVotante').on('show.bs.modal', () => {
                this.ocultarMensaje('#mensajeEditarVotante');
            });
        },

        async crearVotante() {
            const payload = {
                cedula: $('#crearCedula').val().trim(),
                nombreCompleto: $('#crearNombreCompleto').val().trim(),
                fechaNacimiento: $('#crearFechaNacimiento').val(),
                provincia: $('#crearProvincia').val()
            };

            if (!payload.cedula || !payload.nombreCompleto || !payload.fechaNacimiento || !payload.provincia) {
                this.mostrarMensaje('#mensajeCrearVotante', 'Complete todos los campos obligatorios.', 'danger');
                return;
            }

            try {
                const res = await fetch('/Votante/CrearVotante', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();
                if (!res.ok || !data.esCorrecto) {
                    this.mostrarMensaje('#mensajeCrearVotante', data.mensaje || 'No se pudo registrar el votante.', 'danger');
                    return;
                }

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalCrearVotante')).hide();
                this.tabla.ajax.reload(null, false);
                Swal.fire({
                    icon: 'success',
                    title: 'Votante registrado',
                    text: data.mensaje,
                    timer: 1800,
                    showConfirmButton: false
                });
            } catch {
                this.mostrarMensaje('#mensajeCrearVotante', 'Error de comunicación con el servidor.', 'danger');
            }
        },

        async cargarVotante(id) {
            try {
                const res = await fetch(`/Votante/ObtenerVotantePorId/${id}`);
                if (!res.ok) {
                    throw new Error();
                }

                const votante = await res.json();
                $('#editarIdVotante').val(votante.idVotante);
                $('#editarCedula').val(votante.cedula);
                $('#editarNombreCompleto').val(votante.nombreCompleto);
                $('#editarFechaNacimiento').val(votante.fechaNacimiento);
                $('#editarProvincia').val(votante.provincia);

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEditarVotante')).show();
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo cargar la información del votante.'
                });
            }
        },

        async actualizarVotante() {
            const payload = {
                idVotante: parseInt($('#editarIdVotante').val(), 10),
                cedula: $('#editarCedula').val().trim(),
                nombreCompleto: $('#editarNombreCompleto').val().trim(),
                fechaNacimiento: $('#editarFechaNacimiento').val(),
                provincia: $('#editarProvincia').val()
            };

            if (!payload.cedula || !payload.nombreCompleto || !payload.fechaNacimiento || !payload.provincia) {
                this.mostrarMensaje('#mensajeEditarVotante', 'Complete todos los campos obligatorios.', 'danger');
                return;
            }

            try {
                const res = await fetch('/Votante/ActualizarVotante', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();
                if (!res.ok || !data.esCorrecto) {
                    this.mostrarMensaje('#mensajeEditarVotante', data.mensaje || 'No se pudo actualizar el votante.', 'danger');
                    return;
                }

                bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEditarVotante')).hide();
                this.tabla.ajax.reload(null, false);
                Swal.fire({
                    icon: 'success',
                    title: 'Votante actualizado',
                    text: data.mensaje,
                    timer: 1800,
                    showConfirmButton: false
                });
            } catch {
                this.mostrarMensaje('#mensajeEditarVotante', 'Error de comunicación con el servidor.', 'danger');
            }
        },

        confirmarEliminar(id) {
            Swal.fire({
                title: '¿Eliminar votante?',
                text: 'Si el votante ya emitió su voto, la eliminación será bloqueada por integridad referencial.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.eliminarVotante(id);
                }
            });
        },

        async eliminarVotante(id) {
            try {
                const res = await fetch(`/Votante/EliminarVotante/${id}`, {
                    method: 'DELETE'
                });
                const data = await res.json();

                if (!res.ok || !data.esCorrecto) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Operación bloqueada',
                        text: data.mensaje || 'No se pudo eliminar el votante.'
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
                    text: 'Ocurrió un error al intentar eliminar el votante.'
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

    $(document).ready(() => VotanteUI.init());
})();
