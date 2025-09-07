# Sistema de Apoio Acadêmico - TCC UFAM

---

# Sumário

- [Resumo](#resumo)
- [Abstract](#abstract)
- [1. Introdução](#1-introdução)
  - [1.1 Objetivos](#11-objetivos)
    - [1.1.1 Objetivo Geral](#111-objetivo-geral)
    - [1.1.2 Objetivos Específicos](#112-objetivos-específicos)
  - [1.2 Metodologia](#12-metodologia)
  - [1.3 Organização do Trabalho](#13-organização-do-trabalho)
- [2. Referencial Teórico](#2-referencial-teórico)
- [3. Trabalhos Relacionados](#3-trabalhos-relacionados)
  - [3.1 Sistema para Gerenciamento e Agendamento de Consultas para Psicólogos e Clientes](#31-sistema-para-gerenciamento-e-agendamento-de-consultas-para-psicólogos-e-clientes)
  - [3.2 SystemPsi: Sistema Gerenciador para Psicólogos em Atuação Remota](#32-systempsi-sistema-gerenciador-para-psicólogos-em-atuação-remota)
  - [3.3 Sistema para Agendamento de Serviços](#33-sistema-para-agendamento-de-serviços)
- [4. Materiais e Métodos](#4-materiais-e-métodos)
  - [4.1 Levantamento de Requisitos](#41-levantamento-de-requisitos)
    - [4.1.1 Requisitos Funcionais (RF)](#411-requisitos-funcionais-rf)
    - [4.1.2 Requisitos Não Funcionais (RNF)](#412-requisitos-não-funcionais-rnf)
  - [4.2 Tecnologias utilizadas](#42-tecnologias-utilizadas)
  - [4.3 Banco de dados](#43-banco-de-dados)
  - [4.4 User stories e Prototipação](#44-user-stories-e-prototipação)
- [5. Resultados](#5-resultados)
- [6. Considerações finais](#6-considerações-finais)
- [Referências](#referências)
- [Apêndice](#x-apêndice)

---

# Resumo

A crescente demanda por suporte à saúde mental no ensino superior impulsiona a busca por ferramentas tecnológicas que otimizem a gestão dos atendimentos. Visando aprimorar os serviços psicopedagógicos oferecidos na Universidade Federal do Amazonas (UFAM), este trabalho apresenta o desenvolvimento de um Sistema de Apoio Acadêmico. A plataforma web foi projetada para agregar eficiência, segurança e organização ao processo, servindo como uma evolução aos fluxos de trabalho tradicionais. A solução introduz um portal de agendamento online, onde alunos e professores podem visualizar a disponibilidade da equipe e solicitar horários de forma centralizada. Para os profissionais de psicologia, o sistema oferece um painel de gestão que simplifica a administração de consultas, além de um módulo de prontuário eletrônico para o registro seguro e sigiloso das sessões, cujos dados são protegidos por criptografia para garantir a máxima confidencialidade. Como funcionalidade de destaque, o sistema permite que observações pontuais, julgadas relevantes pelos psicólogos para o ambiente de sala de aula, sejam compartilhadas de forma segura. Cada professor pode visualizar apenas as informações pertinentes aos seus próprios alunos, o que promove uma colaboração mais integrada e com maior privacidade no acompanhamento do estudante. O projeto foi desenvolvido seguindo etapas da engenharia de software, incluindo levantamento de requisitos, modelagem de dados e implementação de uma arquitetura moderna. Como resultado, a plataforma eleva a qualidade do serviço de apoio, proporcionando maior agilidade e fortalecendo a conformidade com a Lei Geral de Proteção de Dados (LGPD), utilizando a criptografia como pilar técnico para a segurança das informações e a privacidade dos estudantes. O sistema estabelece, assim, uma ponte de comunicação ética e eficaz entre os psicólogos e o corpo docente, contribuindo para um ambiente acadêmico mais integrado e eficiente.

**Palavras-chave:** apoio psicológico, sistema web, gestão acadêmica, privacidade de dados, engenharia de software.

---

# Abstract

The growing demand for mental health support in higher education drives the search for technological tools that optimize service management. Aiming to enhance the psychopedagogical services offered at the Federal University of Amazonas (UFAM), this work presents the development of an Academic Support System. The web platform is designed to add efficiency, security, and organization to the process, serving as an evolution of traditional workflows. The solution introduces an online scheduling portal where students and professors can view staff availability and request appointments in a centralized manner. For psychology professionals, the system offers a management dashboard that simplifies appointment administration, as well as an electronic records module for the secure and confidential logging of sessions, with its data protected by encryption to ensure maximum confidentiality. As a key feature, the system allows specific observations, deemed relevant by psychologists for the classroom environment, to be shared securely. Each professor can only view information pertaining to their own students, which promotes a more integrated collaboration with greater privacy in student follow-up. The project was developed following software engineering stages, including requirements elicitation, data modeling, and the implementation of a modern architecture. As a result, the platform elevates the quality of the support service, providing greater agility and strengthening compliance with the Brazilian General Data Protection Law (LGPD), using encryption as a technical pillar for information security and student privacy. The system thus establishes an ethical and effective communication bridge between psychologists and faculty, contributing to a more integrated and efficient academic environment.

**Keywords:** psychological support, web system, academic management, data privacy, software engineering.

---

# 1. Introdução

A gestão de serviços de apoio psicopedagógico em universidades como a UFAM demanda ferramentas eficientes. Processos manuais para agendamento, registro e comunicação são frequentemente lentos e apresentam riscos à privacidade dos dados dos estudantes. A tecnologia web oferece uma solução direta para modernizar e proteger esses fluxos de trabalho.

Este trabalho apresenta o desenvolvimento de um Sistema de Apoio Acadêmico, uma plataforma web criada para centralizar e otimizar a gestão dos atendimentos psicológicos no Instituto de Computação (ICOMP). O sistema automatiza o processo de agendamento de consultas, substitui prontuários físicos por um módulo eletrônico com dados protegidos por criptografia e estabelece um canal de comunicação seguro.

O principal diferencial da plataforma é permitir que psicólogos compartilhem observações pontuais e não-clínicas com professores específicos, que podem visualizar informações apenas dos seus próprios alunos. Essa funcionalidade promove um acompanhamento integrado do estudante, mantendo a confidencialidade e o controle de acesso à informação, em conformidade com a Lei Geral de Proteção de Dados (LGPD).

## 1.1 Objetivos

O projeto é guiado por um objetivo principal e desmembrado em metas específicas.

### 1.1.1 Objetivo Geral

Desenvolver um sistema web seguro e eficiente para automatizar o gerenciamento do serviço de apoio psicopedagógico do ICOMP/UFAM, otimizando agendamentos, prontuários eletrônicos e a comunicação controlada entre psicólogos, alunos e professores.

### 1.1.2 Objetivos Específicos

- Levantar os requisitos funcionais e não funcionais do sistema.
- Projetar o banco de dados para o armazenamento seguro das informações.
- Implementar um módulo de agendamento de consultas online.
- Desenvolver um módulo de prontuário eletrônico com criptografia de dados.
- Criar um sistema de perfis de acesso com permissões restritas para cada tipo de usuário (aluno, professor, psicólogo e administrador).
- Assegurar a conformidade do sistema com a Lei Geral de Proteção de Dados (LGPD).
- Validar as funcionalidades da plataforma para garantir que atendam às necessidades dos usuários.

---

## 1.2 Metodologia

A metodologia adotada para o desenvolvimento deste projeto foi baseada em um **modelo de desenvolvimento iterativo e incremental**. Essa abordagem foi escolhida por sua flexibilidade, que permite a construção do software em ciclos (iterações), agregando novas funcionalidades a cada etapa e possibilitando a realização de ajustes com base em validações contínuas.

O processo foi organizado nas seguintes fases sequenciais:

1.  **Pesquisa e Fundamentação Teórica:** A fase inicial consistiu na revisão de literatura para compreender as soluções existentes para a gestão de serviços psicopedagógicos e sistemas de agendamento. Esta etapa incluiu a pesquisa por trabalhos acadêmicos e ferramentas de mercado, cujos resultados são apresentados nos capítulos de Referencial Teórico e Trabalhos Relacionados.

2.  **Levantamento e Análise de Requisitos:** Esta fase focou em definir o que o sistema deveria fazer. Foram realizadas reuniões com a psicóloga do ICOMP para mapear o fluxo de trabalho atual, identificar as principais dores e necessidades. Como resultado, os requisitos do sistema foram documentados em Requisitos Funcionais (RF) e Não Funcionais (RNF), que são detalhados no Capítulo 4.

3.  **Modelagem e Projeto do Sistema (Design):** Com os requisitos definidos, a próxima etapa foi projetar a arquitetura da solução. Isso incluiu:

    - A **modelagem do banco de dados**, com a criação do Modelo Entidade-Relacionamento (MER) para estruturar como as informações seriam armazenadas.
    - A definição da **arquitetura do software**, separando a lógica em backend, frontend e banco de dados.
    - A criação de **protótipos de baixa fidelidade (wireframes)** das principais telas do sistema para visualizar a experiência do usuário (UX) antes da implementação.

4.  **Desenvolvimento Iterativo:** A implementação do código foi dividida em ciclos, focando em entregar módulos funcionais de forma incremental. As principais iterações foram:

    - **Iteração 1: Módulo de Agendamento.** Implementação do calendário público, do formulário de solicitação e do painel de gerenciamento de consultas para o psicólogo.
    - **Iteração 2: Módulo de Prontuários e Perfis.** Desenvolvimento da área logada do psicólogo para criação de prontuários eletrônicos e da área do aluno para visualização de histórico.
    - **Iteração 3: Módulo de Professores e Administrador.** Criação do sistema de permissões, da visualização restrita para professores e das funcionalidades administrativas.

5.  **Validação e Testes:** Ao final de cada iteração, foram realizados testes funcionais para verificar se os requisitos foram atendidos corretamente. O protótipo funcional também foi apresentado à psicóloga e à orientadora para validação, coletando feedbacks que foram utilizados para refinar o sistema na iteração seguinte. Esta abordagem garantiu que o produto final estivesse alinhado com as expectativas e necessidades reais do usuário.

Essa metodologia estruturada permitiu um desenvolvimento controlado, com entregas de valor contínuas e a mitigação de riscos, assegurando que o projeto avançasse de forma organizada e eficiente.

## 1.3 Organização do Trabalho

# 2. Referencial Teórico

# 3. Trabalhos Relacionados

Neste capítulo, são apresentados trabalhos acadêmicos e projetos de software que abordam temas correlatos ao deste TCC, como sistemas de agendamento online, gerenciamento de consultas psicológicas e plataformas de apoio ao estudante. A análise destes trabalhos permite identificar funcionalidades consolidadas, tecnologias empregadas e, fundamentalmente, as lacunas que justificam o desenvolvimento da presente proposta.

## 3.1 Sistema para Gerenciamento e Agendamento de Consultas para Psicólogos e Clientes

Desenvolvido no âmbito de um Trabalho de Conclusão de Curso no Instituto Federal de Goiás (IFG), este projeto de Souza (2021) propõe uma plataforma web para a gestão de agendamentos de consultas psicológicas. O sistema inclui funcionalidades essenciais como agendamento, registro de anamneses e geração de relatórios, operando com perfis de usuário distintos para o profissional e para o cliente. As tecnologias utilizadas foram PHP, JavaScript, HTML, CSS e MySQL.

A principal relevância deste trabalho reside na sua abordagem direta ao agendamento online no contexto da psicologia, validando a arquitetura de perfis de usuário (psicólogo e paciente) que é similar à base do nosso sistema (psicólogo e aluno). No entanto, sua proposta é focada na relação dual cliente-profissional. O diferencial do nosso projeto se manifesta na introdução de um terceiro perfil, o de professor, e na criação de um ecossistema integrado ao ambiente acadêmico, com um canal de comunicação específico e controlado, funcionalidade não abordada por este trabalho.

**Referência:** SOUZA, Wesley Queiroz de. **Sistema para gerenciamento e agendamento de consultas para psicólogos e clientes: atendimentos online em meio a pandemia do Covid-19**. 2021. TCC (Bacharelado em Sistemas de Informação) - Instituto Federal de Goiás, Câmpus Jataí. Disponível em: [https://repositorio.ifg.edu.br/handle/prefix/1726](https://repositorio.ifg.edu.br/handle/prefix/1726).

## 3.2 SystemPsi: Sistema Gerenciador para Psicólogos em Atuação Remota

O SystemPsi é uma ferramenta tecnológica apresentada como artigo no Congresso Latino-Americano de Software Livre e Tecnologias Abertas (Latinoware), desenvolvida no Instituto Federal Catarinense (IFC). O sistema foi projetado para auxiliar psicólogos em atendimentos remotos, e seu desenvolvimento contou com a validação de um profissional da área para garantir a pertinência das funcionalidades. A pilha de tecnologias inclui PHP e MySQL.

Este trabalho se destaca pela sua metodologia, que enfatiza a importância da validação das funcionalidades junto ao usuário final — o profissional de psicologia. Essa abordagem reforça a metodologia adotada em nosso projeto, que também se baseia na colaboração direta com a psicóloga do ICOMP. O SystemPsi, contudo, concentra-se nas necessidades do psicólogo em um contexto de atuação remota geral. Nossa proposta avança ao especializar a ferramenta para o nicho universitário, atendendo não apenas às necessidades do psicólogo, mas também às dinâmicas de interação com alunos e professores dentro de uma instituição de ensino.

**Referência:** STEFEN, L. E. et al. **SystemPsi: Sistema Gerenciador para Psicólogos em Atuação Remota**. In: Anais do Congresso Latino-Americano de Software Livre e Tecnologias Abertas, 2022. Disponível em: [https://sol.sbc.org.br/index.php/latinoware/article/view/26074](https://sol.sbc.org.br/index.php/latinoware/article/view/26074).

## 3.3 Sistema para Agendamento de Serviços

Em seu Trabalho de Conclusão de Curso na Universidade Tecnológica Federal do Paraná (UTFPR), Kieras (2019) detalha o desenvolvimento de um sistema genérico para o agendamento de serviços, composto por uma aplicação web e um aplicativo móvel.

A principal contribuição deste trabalho para o nosso projeto não está no domínio da aplicação, mas sim na sua sólida documentação de engenharia de software. O detalhamento da análise de requisitos, dos casos de uso e do planejamento das funcionalidades serve como uma excelente referência metodológica para a estruturação do presente TCC. Enquanto o sistema da UTFPR foi projetado para ser genérico e aplicável a diversos contextos de agendamento, nosso sistema se aprofunda em um domínio específico — o apoio psicopedagógico universitário. Essa especialização permite a criação de funcionalidades sob medida, como o prontuário eletrônico e a visualização restrita para professores, que não fariam parte de uma solução genérica.

**Referência:** KIERAS, Ramon Wolski. **Sistema para Agendamento de Serviços**. 2019. TCC (Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas) - Universidade Tecnológica Federal do Paraná, Campo Mourão. Disponível em: [http://repositorio.utfpr.edu.br/jspui/bitstream/1/16826/1/PG_COADS_2019_1_02.pdf](http://repositorio.utfpr.edu.br/jspui/bitstream/1/16826/1/PG_COADS_2019_1_02.pdf).

---

# 4. Materiais e Métodos

Este capítulo detalha os materiais, as ferramentas e os procedimentos metodológicos empregados no desenvolvimento do Sistema de Apoio Acadêmico. Serão abordados o processo de levantamento de requisitos, as tecnologias selecionadas para a implementação e a modelagem do banco de dados que estrutura a aplicação.

## 4.1 Levantamento de Requisitos

A etapa de levantamento de requisitos foi fundamental para definir o escopo e as funcionalidades do sistema. O processo foi conduzido através de reuniões com a psicóloga do ICOMP, a principal interessada (stakeholder) no projeto, e com a orientação da professora Dra. Ana Carolina Oran. Nessas reuniões, foi possível mapear o fluxo de trabalho existente, identificar as limitações do método atual e especificar as necessidades dos futuros usuários (alunos, professores e a própria psicóloga).

As funcionalidades desejadas foram então estruturadas e classificadas em duas categorias principais, conforme a prática da Engenharia de Software: Requisitos Funcionais e Requisitos Não Funcionais.

### 4.1.1 Requisitos Funcionais (RF)

Os Requisitos Funcionais descrevem as ações e funcionalidades que o sistema deve ser capaz de executar. Eles definem o comportamento do software sob a perspectiva do usuário. A Tabela 1 apresenta a lista de requisitos funcionais identificados para o projeto.

**Tabela 1 – Requisitos Funcionais do Sistema**

| ID    | Categoria                  | Descrição                                                                                                              |
| :---- | :------------------------- | :--------------------------------------------------------------------------------------------------------------------- |
| RF001 | Agendamento Público        | Qualquer usuário (visitante, aluno, professor) deve poder acessar uma página de agendamento.                           |
| RF002 | Agendamento Público        | O sistema deve permitir a seleção de um psicólogo específico para visualização da agenda.                              |
| RF003 | Agendamento Público        | Ao visualizar a agenda, o sistema deve exibir o nome e o instituto do psicólogo (ex: "Ana Machado - ICOMP").           |
| RF004 | Agendamento Público        | O sistema deve exibir o calendário do psicólogo, mostrando claramente os dias e horários disponíveis.                  |
| RF005 | Agendamento Público        | O sistema deve marcar visualmente os horários com solicitações pendentes ou confirmadas, impedindo a seleção.          |
| RF006 | Solicitação de Agendamento | Um visitante deve poder preencher um formulário com seus dados (nome, matrícula, email) para solicitar um agendamento. |
| RF007 | Solicitação de Agendamento | Um visitante deve poder solicitar um agendamento para um colega, informando os dados de ambos.                         |
| RF008 | Solicitação de Agendamento | Um aluno logado deve ter seus dados preenchidos automaticamente ao solicitar um agendamento para si.                   |
| RF009 | Solicitação de Agendamento | Um aluno logado deve poder solicitar um agendamento para um colega, e o sistema deve registrar o solicitante.          |
| RF010 | Solicitação de Agendamento | Um professor logado deve poder solicitar um agendamento para um aluno, e o sistema deve registrar o solicitante.       |
| RF011 | Solicitação de Agendamento | Ao ser solicitado, um horário deve ser imediatamente bloqueado na agenda pública, mesmo que pendente de confirmação.   |
| RF012 | Solicitação de Agendamento | O sistema deve enviar um email de confirmação de recebimento da solicitação.                                           |
| RF013 | Gerenciamento (Psicólogo)  | O psicólogo deve ter um painel para visualizar todas as solicitações de agendamento pendentes.                         |
| RF014 | Gerenciamento (Psicólogo)  | O psicólogo deve poder visualizar os detalhes de cada solicitação.                                                     |
| RF015 | Gerenciamento (Psicólogo)  | O psicólogo deve poder aprovar uma solicitação, mudando seu status para "Confirmado".                                  |
| RF016 | Gerenciamento (Psicólogo)  | O psicólogo deve poder recusar uma solicitação, preenchendo uma justificativa obrigatória.                             |
| RF017 | Gerenciamento (Psicólogo)  | O sistema deve notificar por email a aprovação do agendamento ao paciente e ao solicitante.                            |
| RF018 | Gerenciamento (Psicólogo)  | O sistema deve notificar por email a recusa do agendamento, incluindo a justificativa.                                 |
| RF019 | Gerenciamento (Psicólogo)  | Se uma solicitação for recusada, o horário deve voltar a ficar disponível na agenda.                                   |
| RF020 | Gerenciamento (Psicólogo)  | O psicólogo deve poder criar um agendamento diretamente em sua agenda, que já nasce como "Confirmado".                 |
| RF021 | Administração              | O administrador deve ter acesso a um painel de controle centralizado.                                                  |
| RF022 | Administração              | O administrador deve poder criar, visualizar, editar e desativar contas de usuários.                                   |
| RF023 | Administração              | O administrador deve poder atribuir ou alterar o perfil de um usuário.                                                 |
| RF024 | Administração              | O administrador deve poder configurar a disponibilidade de cada psicólogo.                                             |
| RF025 | Administração              | O administrador deve poder visualizar todas as solicitações de agendamento do sistema.                                 |
| RF026 | Administração              | O administrador deve poder aprovar uma solicitação em nome de um psicólogo, com o registro da ação.                    |
| RF027 | Administração              | O administrador deve poder recusar uma solicitação em nome de um psicólogo, com o registro da ação.                    |
| RF028 | Administração              | O administrador deve poder gerar relatórios estatísticos básicos (sem dados sigilosos).                                |
| RF029 | Administração              | O administrador deve poder consultar um log de atividades importantes do sistema.                                      |
| RF030 | Gerenciamento (Psicólogo)  | O psicólogo deve poder criar e editar um prontuário eletrônico sigiloso para cada sessão.                              |
| RF031 | Gerenciamento (Psicólogo)  | O psicólogo deve poder acessar o histórico de consultas realizadas.                                                    |
| RF032 | Gerenciamento (Psicólogo)  | O psicólogo deve poder visualizar os detalhes completos de uma consulta passada, incluindo o prontuário.               |
| RF033 | Área do Aluno              | Um aluno logado deve poder visualizar seu histórico de atendimentos passados (data, hora, profissional).               |
| RF034 | Gerenciamento (Psicólogo)  | O psicólogo deve poder criar, editar e excluir categorias de atendimento (ex: "Primeira consulta").                    |
| RF035 | Solicitação de Agendamento | O solicitante deve selecionar uma categoria de atendimento ao fazer o pedido.                                          |
| RF036 | Gerenciamento (Psicólogo)  | A categoria do atendimento deve ser exibida nos detalhes da consulta para o psicólogo.                                 |

### 4.1.2 Requisitos Não Funcionais (RNF)

Os Requisitos Não Funcionais especificam os critérios de qualidade e as restrições operacionais do sistema. Eles não se referem a uma funcionalidade específica, mas sim a como o sistema deve operar em termos de desempenho, segurança, usabilidade, entre outros. A Tabela 2 detalha os requisitos não funcionais do projeto.

**Tabela 2 – Requisitos Não Funcionais do Sistema**

| ID     | Categoria        | Descrição                                                                                                                      |
| :----- | :--------------- | :----------------------------------------------------------------------------------------------------------------------------- |
| RNF001 | Tecnologia       | O sistema deve ser uma aplicação web, acessível através dos principais navegadores modernos.                                   |
| RNF002 | Usabilidade      | A interface do sistema deve ser responsiva, adaptando-se a desktops, tablets e smartphones.                                    |
| RNF003 | Segurança        | O acesso dos usuários cadastrados deve ser controlado por um sistema de autenticação.                                          |
| RNF004 | Segurança / LGPD | Os dados de pacientes devem ser armazenados de forma segura e criptografada, com acesso restrito e em conformidade com a LGPD. |
| RNF005 | Confiabilidade   | O sistema deve ter uma disponibilidade de 99% (excluindo manutenções planejadas).                                              |
| RNF006 | Desempenho       | As páginas principais devem carregar em no máximo 3 segundos em uma conexão padrão.                                            |
| RNF007 | Confiabilidade   | O sistema deve garantir a entrega de emails de notificação, tratando possíveis falhas no envio.                                |
| RNF008 | Usabilidade      | A interface deve ser clara e intuitiva, permitindo o uso sem a necessidade de um manual.                                       |
| RNF009 | Segurança        | Todas as ações críticas do administrador devem ser registradas em um log de auditoria.                                         |
| RNF010 | Manutenibilidade | O psicólogo deve poder gerenciar categorias de atendimento sem a necessidade de alteração no código-fonte.                     |

## 4.2 Tecnologias utilizadas

## 4.3 Banco de dados

## 4.4 User stories e Prototipação

# 5. Resultados

# 6. Considerações finais

# Referências

# X Apêndice
