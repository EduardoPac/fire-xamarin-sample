# Fire Xamarin

Este projeto é uma aplicação de teste para integração com Firebase, todo o CRUD é feito conectando diretamente com o banco de Realtime Database do proprio Firebase.

Funcionalidades basicas do app:
- Cadastro de contato
- Edição de contato
- Remoção de contato

Funcionalidades adicionais:
- Cadastrar endereço com o uso do GPS do dispositivo
- Realizar ligações a partir do numero cadastrado
- Enviar email a partir do email cadastrado
- Abrir o mapa para traçar uma rota a partir do endereço cadastrado

### No app foi usados as seguintes ferramentas
- Xamarin.Forms
- FirebaseDatabase.net - Firebase
- Xamarin.Essentials - Geolocalização e realização das funcionalidades nativas de chamada e email 
- Rg.Plugins.Popup - Criação dos Bottonsheets para o Android
- Xamarin.Forms.BehaviorValidationPack - Mascaras de telefone e validação de cadastro de email
- BurgerMonkeys.Tools
Testes unitários
- Xunit 
- Bogus - Mock dos dados
- FluentAssertions - Tratativa dos asserts dos testes unitarios 

### Screenshots
Android
|   1   |  2  |    3    |
| :---:         |     :---:      |          :---: |
| ![android 1](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/android1.jpeg)  | ![android 2](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/android2.jpeg)  | ![android 3](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/android3.jpeg)    |

iOS
|   1   |  2  |    3    |
| :---:         |     :---:      |          :---: |
| ![iOS 1](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/ios1.png) | ![iOS 2](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/ios2.png) | ![iOS 3](https://github.com/EduardoPac/fire-xamarin-sample/blob/main/screenshots/ios3.png)  |

### Features futuras
- Banco local para uso offline
- Temas
