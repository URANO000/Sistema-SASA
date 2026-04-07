namespace BusinessLogic.Servicios.Correo.Plantillas;

public static class PlantillasCorreo
{
    // Plantilla 100% HTML, sin Graph, sin MVC
    public static string ConfirmacionEmpleado(string nombreEmpleado, string tipoPermiso)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>

<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>

        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>

          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;letter-spacing:.2px;'>
                Sistema de Gestión SASA
              </div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>
                Notificación automática
              </div>
            </td>
          </tr>

          <tr>
            <td style='padding:28px 28px 8px 28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin:0 0 10px 0;'>
                Confirmación de permiso
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin:0 0 14px 0;'>
                A quien corresponda, <b>{nombreEmpleado}</b>
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 14px 0;'>
                Mediante la presente notificación por correo electrónico, se confirma la creación de un permiso del tipo
                <b>{tipoPermiso}</b>, el cual ha sido registrado correctamente en el sistema.
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 18px 0;'>
                Si deseas observar más información, ingresa al sistema de Recursos Humanos para consultar el detalle del permiso.
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:10px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='https://ticenter.sistemasanaliticos.cr/'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Ir al sistema
                    </a>
                  </td>
                </tr>
              </table>

              <div style='height:1px;background:#eef2f7;margin:18px 0;'></div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 4px 0;'>
                Gracias por tu colaboración,
              </div>
              <div style='font-size:14px;line-height:1.7;color:#111827;font-weight:700;margin:0 0 6px 0;'>
                Sistemas Analíticos
              </div>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;margin:0;'>
                Este es un mensaje automático. Por favor, no respondas a este correo.
              </div>
            </td>
          </tr>

          <tr>
            <td style='background:#012473;height:6px;line-height:6px;font-size:0;'>&nbsp;</td>
          </tr>

        </table>

      </td>
    </tr>
  </table>
</body>
</html>";
    }
    public static string ActivacionCuenta(string nombreUsuario, string activationLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>

<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>

        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>

          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;letter-spacing:.2px;'>
                Sistema de Gestión SASA
              </div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>
                Activación de cuenta
              </div>
            </td>
          </tr>

          <tr>
            <td style='padding:28px 28px 8px 28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin:0 0 10px 0;'>
                Activa tu cuenta
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin:0 0 14px 0;'>
                Hola, <b>{nombreUsuario}</b>
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 14px 0;'>
                Tu usuario fue creado en el <b>Sistema de Gestión SASA</b>. Para activar tu cuenta y crear tu contraseña, haz clic en el botón de abajo.
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:10px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{activationLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Activar cuenta
                    </a>
                  </td>
                </tr>
              </table>

              <div style='height:1px;background:#eef2f7;margin:18px 0;'></div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 4px 0;'>
                Gracias,
              </div>
              <div style='font-size:14px;line-height:1.7;color:#111827;font-weight:700;margin:0 0 6px 0;'>
                Sistemas Analíticos
              </div>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;margin:0;'>
                Este es un mensaje automático. Por favor, no respondas a este correo.
              </div>
            </td>
          </tr>

          <tr>
            <td style='background:#012473;height:6px;line-height:6px;font-size:0;'>&nbsp;</td>
          </tr>

        </table>

      </td>
    </tr>
  </table>
</body>
</html>";
    }

    public static string RecuperacionContrasena(string nombreUsuario, string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>

<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>

        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>

          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;letter-spacing:.2px;'>
                Sistema de Gestión SASA
              </div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>
                Recuperación de contraseña
              </div>
            </td>
          </tr>

          <tr>
            <td style='padding:28px 28px 8px 28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin:0 0 10px 0;'>
                Restablecer contraseña
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin:0 0 14px 0;'>
                Hola, <b>{nombreUsuario}</b>
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 14px 0;'>
                Recibimos una solicitud para restablecer tu contraseña. Haz clic en el botón para continuar.
                Si no fuiste tú, puedes ignorar este correo.
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:10px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{resetLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Restablecer contraseña
                    </a>
                  </td>
                </tr>
              </table>

              <div style='height:1px;background:#eef2f7;margin:18px 0;'></div>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;margin:0;'>
                Este es un mensaje automático. Por favor, no respondas a este correo.
              </div>
            </td>
          </tr>

          <tr>
            <td style='background:#012473;height:6px;line-height:6px;font-size:0;'>&nbsp;</td>
          </tr>

        </table>

      </td>
    </tr>
  </table>
</body>
</html>";
    }

    public static string TiqueteCreado(string nombreUsuario, int tiqueteId, string asunto, string detalleLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>

<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>

        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>

          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;letter-spacing:.2px;'>
                Sistema de Gestión SASA
              </div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>
                Notificación automática
              </div>
            </td>
          </tr>

          <tr>
            <td style='padding:28px 28px 8px 28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin:0 0 10px 0;'>
                Se ha creado tu tiquete
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin:0 0 14px 0;'>
                Hola, <b>{nombreUsuario}</b>
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin:0 0 14px 0;'>
                Tu tiquete <b>#{tiqueteId}</b> ha sido creado con el asunto: <b>{asunto}</b>.
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:10px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{detalleLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Ver tiquete
                    </a>
                  </td>
                </tr>
              </table>

              <div style='height:1px;background:#eef2f7;margin:18px 0;'></div>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;margin:0;'>
                Este es un mensaje automático. Por favor, no respondas a este correo.
              </div>
            </td>
          </tr>

          <tr>
            <td style='background:#012473;height:6px;line-height:6px;font-size:0;'>&nbsp;</td>
          </tr>

        </table>

      </td>
    </tr>
  </table>
</body>
</html>";
    }
    public static string TiqueteCreadoAdmins(int tiqueteId, string asunto, string nombreReportador, string correoReportador, string detalleLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>
<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>
        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>
          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;'>Sistema de Gestión SASA</div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>Notificación automática</div>
            </td>
          </tr>
          <tr>
            <td style='padding:28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin-bottom:14px;'>
                Nuevo tiquete pendiente de revisión
              </div>

              <div style='font-size:14px;line-height:1.7;color:#4b5563;margin-bottom:10px;'>
                Se ha creado un nuevo tiquete en el sistema.
              </div>

              <div style='font-size:14px;line-height:1.8;color:#111827;'>
                <b>ID:</b> #{tiqueteId}<br/>
                <b>Asunto:</b> {asunto}<br/>
                <b>Usuario:</b> {nombreReportador}<br/>
                <b>Correo:</b> {correoReportador}
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:20px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{detalleLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Ver detalle del tiquete
                    </a>
                  </td>
                </tr>
              </table>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;'>
                Este es un mensaje automático enviado desde una cuenta no-reply. Por favor, no responder este correo.
              </div>
            </td>
          </tr>
          <tr>
            <td style='background:#012473;height:6px;font-size:0;line-height:6px;'>&nbsp;</td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }

    public static string TiqueteResueltoOCerrado(string nombreUsuario, int tiqueteId, string asunto, string estado, string detalleLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>
<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>
        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>
          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;'>Sistema de Gestión SASA</div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>Actualización de tiquete</div>
            </td>
          </tr>
          <tr>
            <td style='padding:28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin-bottom:14px;'>
                Su tiquete ha sido actualizado
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin-bottom:14px;'>
                Hola, <b>{nombreUsuario}</b>
              </div>

              <div style='font-size:14px;line-height:1.8;color:#111827;'>
                El tiquete <b>#{tiqueteId}</b> con asunto <b>{asunto}</b> ahora se encuentra en estado <b>{estado}</b>.
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:20px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{detalleLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Ver detalle
                    </a>
                  </td>
                </tr>
              </table>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;'>
                Este es un mensaje automático enviado desde una cuenta no-reply. Por favor, no responder este correo.
              </div>
            </td>
          </tr>
          <tr>
            <td style='background:#012473;height:6px;font-size:0;line-height:6px;'>&nbsp;</td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }

    public static string NuevoAvanceTiquete(string nombreUsuario, int tiqueteId, string asunto, string textoAvance, string detalleLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Sistema de Gestión SASA</title>
</head>
<body style='margin:0;padding:0;background-color:#e8e8e8;font-family:Montserrat, -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
  <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#e8e8e8;'>
    <tr>
      <td align='center' style='padding:40px 16px;'>
        <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0'
               style='width:600px;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 10px 30px rgba(0,0,0,.10);'>
          <tr>
            <td style='background:#012473;padding:18px 28px;'>
              <div style='color:#ffffff;font-size:16px;font-weight:700;'>Sistema de Gestión SASA</div>
              <div style='color:rgba(255,255,255,.85);font-size:12px;margin-top:4px;'>Actualización de tiquete</div>
            </td>
          </tr>
          <tr>
            <td style='padding:28px;'>
              <div style='font-size:18px;font-weight:700;color:#111827;margin-bottom:14px;'>
                Nuevo avance en su tiquete
              </div>

              <div style='font-size:14px;line-height:1.7;color:#374151;margin-bottom:14px;'>
                Hola, <b>{nombreUsuario}</b>
              </div>

              <div style='font-size:14px;line-height:1.8;color:#111827;margin-bottom:12px;'>
                Se agregó un nuevo avance al tiquete <b>#{tiqueteId}</b> - <b>{asunto}</b>.
              </div>

              <div style='font-size:14px;line-height:1.8;color:#4b5563;background:#f8fafc;border-radius:10px;padding:14px;'>
                {textoAvance}
              </div>

              <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:20px 0 18px 0;'>
                <tr>
                  <td align='center' style='border-radius:999px;background:#012473;'>
                    <a href='{detalleLink}'
                       style='display:inline-block;padding:12px 22px;font-size:14px;color:#ffffff;text-decoration:none;font-weight:700;border-radius:999px;'>
                      Ver detalle
                    </a>
                  </td>
                </tr>
              </table>

              <div style='font-size:12px;line-height:1.6;color:#6b7280;'>
                Este es un mensaje automático enviado desde una cuenta no-reply. Por favor, no responder este correo.
              </div>
            </td>
          </tr>
          <tr>
            <td style='background:#012473;height:6px;font-size:0;line-height:6px;'>&nbsp;</td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }

}
