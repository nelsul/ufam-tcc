using IcompCare.Application.Interfaces;

namespace IcompCare.Infrastructure.Email;

public class EmailTemplateService : IEmailTemplateService
{
    private const string BaseLayoutTemplate =
        @"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{0}</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #5E5340;
            background-color: #f5f5f0;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #FDFDF5;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background: linear-gradient(135deg, #78D879 0%, #5BC75D 100%);
            padding: 32px;
            text-align: center;
        }}
        .email-header h1 {{
            color: white;
            margin: 0;
            font-size: 24px;
            font-weight: 700;
        }}
        .email-body {{
            padding: 32px;
        }}
        .email-footer {{
            background-color: #f0efe8;
            padding: 24px 32px;
            text-align: center;
            font-size: 12px;
            color: #5E5340;
            opacity: 0.7;
        }}
        .btn {{
            display: inline-block;
            background: linear-gradient(135deg, #F6A95B 0%, #F59542 100%);
            color: white;
            padding: 12px 24px;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 600;
            margin: 16px 0;
        }}
        .btn:hover {{
            opacity: 0.9;
        }}
        .info-box {{
            background-color: #f0efe8;
            border-radius: 8px;
            padding: 16px;
            margin: 16px 0;
        }}
        .info-label {{
            font-size: 12px;
            color: #F6A95B;
            text-transform: uppercase;
            font-weight: 700;
            margin-bottom: 4px;
        }}
        .info-value {{
            font-size: 16px;
            font-weight: 600;
            color: #5E5340;
        }}
        .highlight {{
            color: #78D879;
            font-weight: 600;
        }}
        .warning {{
            color: #F6A95B;
            font-weight: 600;
        }}
    </style>
</head>
<body>
    <div style=""padding: 20px;"">
        <div class=""email-container"">
            <div class=""email-header"">
                <h1>IcompCare</h1>
            </div>
            <div class=""email-body"">
                {1}
            </div>
            <div class=""email-footer"">
                <p>Este é um email automático do sistema IcompCare.</p>
                <p>Instituto de Computação - UFAM</p>
            </div>
        </div>
    </div>
</body>
</html>";

    public string GetBaseLayout(string content, string title)
    {
        return string.Format(BaseLayoutTemplate, title, content);
    }

    public string RenderTemplate(string templateName, object model)
    {
        var template = GetTemplate(templateName);
        return ReplaceTokens(template, model);
    }

    private string GetTemplate(string templateName)
    {
        return templateName switch
        {
            "AppointmentRequested" => AppointmentRequestedTemplate,
            "StudentAppointmentRequested" => StudentAppointmentRequestedTemplate,
            "AppointmentConfirmed" => AppointmentConfirmedTemplate,
            "AppointmentRejected" => AppointmentRejectedTemplate,
            "AppointmentCancelled" => AppointmentCancelledTemplate,
            "AppointmentRescheduled" => AppointmentRescheduledTemplate,
            "PasswordReset" => PasswordResetTemplate,
            "Welcome" => WelcomeTemplate,
            _ => throw new ArgumentException($"Template '{templateName}' not found"),
        };
    }

    private string ReplaceTokens(string template, object model)
    {
        var result = template;
        var properties = model.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var token = $"{{{{{prop.Name}}}}}";
            var value = prop.GetValue(model)?.ToString() ?? string.Empty;
            result = result.Replace(token, value);
        }

        return result;
    }

    private const string AppointmentRequestedTemplate =
        @"
<h2>Nova Solicitação de Agendamento</h2>
<p>Olá, <strong>{{ProfessionalName}}</strong>!</p>
<p>Você recebeu uma nova solicitação de agendamento.</p>

<div class=""info-box"">
    <div class=""info-label"">Solicitante</div>
    <div class=""info-value"">{{RequesterName}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Data e Hora</div>
    <div class=""info-value"">{{AppointmentDate}} às {{AppointmentTime}}</div>
</div>

{{#if Reason}}
<div class=""info-box"">
    <div class=""info-label"">Motivo</div>
    <div class=""info-value"">{{Reason}}</div>
</div>
{{/if}}

<p>Acesse o sistema para revisar e responder à solicitação.</p>
<a href=""{{DashboardUrl}}"" class=""btn"">Acessar Dashboard</a>
";

    private const string StudentAppointmentRequestedTemplate =
        @"
<h2>Solicitação de Agendamento Recebida</h2>
<p>Olá, <strong>{{StudentName}}</strong>!</p>
<p>Recebemos sua solicitação de agendamento com sucesso.</p>

<div class=""info-box"">
    <div class=""info-label"">Profissional</div>
    <div class=""info-value"">{{ProfessionalName}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Data e Hora Solicitada</div>
    <div class=""info-value"">{{AppointmentDate}} às {{AppointmentTime}}</div>
</div>

<p><span class=""warning"">Aguarde a confirmação do profissional.</span></p>
<p>Você receberá um novo email assim que o profissional analisar e confirmar ou reagendar seu atendimento.</p>

<div class=""info-box"" style=""background-color: #fff3cd; border-left: 4px solid #F6A95B;"">
    <p style=""margin: 0; font-size: 14px;"">
        <strong>Importante:</strong> Este agendamento ainda não está confirmado. 
        Por favor, aguarde o retorno do profissional antes de comparecer.
    </p>
</div>
";

    private const string AppointmentConfirmedTemplate =
        @"
<h2>Agendamento Confirmado! ✓</h2>
<p>Olá, <strong>{{PatientName}}</strong>!</p>
<p>Seu agendamento foi <span class=""highlight"">confirmado</span>.</p>

<div class=""info-box"">
    <div class=""info-label"">Profissional</div>
    <div class=""info-value"">{{ProfessionalName}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Data e Hora</div>
    <div class=""info-value"">{{AppointmentDate}} às {{AppointmentTime}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Duração</div>
    <div class=""info-value"">{{Duration}} minutos</div>
</div>

<p>Por favor, chegue com alguns minutos de antecedência.</p>
<p>Em caso de necessidade de cancelamento, entre em contato com antecedência.</p>
";

    private const string AppointmentRejectedTemplate =
        @"
<h2>Agendamento Não Aprovado</h2>
<p>Olá, <strong>{{PatientName}}</strong>!</p>
<p>Infelizmente, sua solicitação de agendamento <span class=""warning"">não pôde ser aprovada</span>.</p>

<div class=""info-box"">
    <div class=""info-label"">Data Solicitada</div>
    <div class=""info-value"">{{AppointmentDate}} às {{AppointmentTime}}</div>
</div>

{{#if RejectionReason}}
<div class=""info-box"">
    <div class=""info-label"">Motivo</div>
    <div class=""info-value"">{{RejectionReason}}</div>
</div>
{{/if}}

<p>Você pode solicitar um novo agendamento em outro horário disponível.</p>
<a href=""{{ScheduleUrl}}"" class=""btn"">Agendar Novo Horário</a>
";

    private const string AppointmentCancelledTemplate =
        @"
<h2>Agendamento Cancelado</h2>
<p>Olá, <strong>{{RecipientName}}</strong>!</p>
<p>O agendamento abaixo foi <span class=""warning"">cancelado</span>.</p>

<div class=""info-box"">
    <div class=""info-label"">Data</div>
    <div class=""info-value"">{{AppointmentDate}} às {{AppointmentTime}}</div>
</div>

<p>Entre em contato caso precise reagendar.</p>
";

    private const string AppointmentRescheduledTemplate =
        @"
<h2>Agendamento Remarcado</h2>
<p>Olá, <strong>{{PatientName}}</strong>!</p>
<p>Seu agendamento foi <span class=""warning"">remarcado</span> pelo profissional.</p>

<div class=""info-box"">
    <div class=""info-label"">Data e Hora Anterior</div>
    <div class=""info-value"">{{OldAppointmentDate}} às {{OldAppointmentTime}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Nova Data e Hora</div>
    <div class=""info-value"">{{NewAppointmentDate}} às {{NewAppointmentTime}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Profissional</div>
    <div class=""info-value"">{{ProfessionalName}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Duração</div>
    <div class=""info-value"">{{Duration}} minutos</div>
</div>

<p>Por favor, confirme sua disponibilidade para o novo horário.</p>
<p><strong>Importante:</strong> Se não puder comparecer no novo horário, entre em contato com o profissional o mais breve possível.</p>
";

    private const string PasswordResetTemplate =
        @"
<h2>Redefinição de Senha</h2>
<p>Olá, <strong>{{UserName}}</strong>!</p>
<p>Sua senha foi redefinida pelo administrador do sistema.</p>

<div class=""info-box"">
    <div class=""info-label"">Nova Senha Temporária</div>
    <div class=""info-value"">{{TemporaryPassword}}</div>
</div>

<p><strong>Importante:</strong> Por segurança, recomendamos que você altere sua senha após o primeiro acesso.</p>
<a href=""{{LoginUrl}}"" class=""btn"">Acessar Sistema</a>
";

    private const string WelcomeTemplate =
        @"
<h2>Bem-vindo ao IcompCare! 🎉</h2>
<p>Olá, <strong>{{UserName}}</strong>!</p>
<p>Sua conta foi criada com sucesso no sistema IcompCare.</p>

<div class=""info-box"">
    <div class=""info-label"">Email</div>
    <div class=""info-value"">{{Email}}</div>
</div>

<div class=""info-box"">
    <div class=""info-label"">Senha Temporária</div>
    <div class=""info-value"">{{TemporaryPassword}}</div>
</div>

<p><strong>Importante:</strong> Por segurança, recomendamos que você altere sua senha após o primeiro acesso.</p>
<a href=""{{LoginUrl}}"" class=""btn"">Acessar Sistema</a>
";
}
