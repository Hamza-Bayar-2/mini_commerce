# Documentation

Bu dokümantasyon; projede tercih edilen teknoloji yığınını, kullanılan araçları ve tasarım desenlerini; ayrıca bu bileşenlerin sistem mimarisine nasıl entegre edildiğini detaylandırmaktadır.

## ٍServisler (Current Services)
- **Auth Service** — JWT token üretimi, refresh token
- **Product Service** — CQRS, Redis cache, event publishing
- **Log Service** — RabbitMQ consumer, merkezi loglama
- **Gateway** — YARP reverse proxy, rate limiting, merkezi auth

## Tech Stack

| Katman | Teknoloji |
|--------|-----------|
| Framework | .NET 10, ASP.NET Core |
| ORM | Entity Framework Core |
| Database | SQL Server (T-SQL) |
| Cache | Redis |
| Messaging | RabbitMQ + MassTransit |
| Gateway | YARP Reverse Proxy |
| Auth | JWT Bearer + BCrypt |
| Logging | Serilog + Seq |
| Validation | FluentValidation |
| Mediator | MediatR (CQRS) |

## Tasarım Desenleri (Design Patterns)

- **Onion Architecture** — Bağımlılıkların İçe Doğru Akması
  - **Amaç:** Uygulama içerisindeki sorumlulukları (sunum, iş kuralları, veri erişimi) izole katmanlara ayırmak ve dış katmanların (Infrastructure, API) yalnızca iç katmanlara (Domain, Application) bağımlı olmasını sağlamak.
  - **Avantajı:** İş kurallarının (Domain) dış dünyadaki teknolojik değişimlerden (Framework, Veritabanı) etkilenmesini önler. Test edilebilirlik (Unit Test) artar. Sorumlulukların ayrılması ile (Separation of Concerns) kodun okunabilirliği ve bakımı çok daha kolay hale gelir.

- **CQRS + MediatR** — Komut ve Sorgu Ayrımı
  - **Amaç:** Veritabanından veri okuma işlemleri (Query - Get) ile veritabanını değiştiren işlemleri (Command - Post/Put/Delete) birbirinden tamamen ayırmak.
  - **Avantajı:** Okuma ve yazma işlemlerinin farklı veri modelleri ve performans gereksinimlerine göre bağımsız olarak optimize edilebilmesini sağlar. Sınıflar küçülür (Single Responsibility). MediatR kullanımı sayesinde ise Controller sınıfları tamamen zayıflar (Thin Controllers) ve iş mantığı (business logic) Handler sınıfları içerisinde encapsulate edilir.

- **Result Pattern** — Hata Kontrolü ve Akış Yönetimi
  - **Amaç:** Application katmanında oluşan hataları veya başarılı durumları standart bir yapı içinde (`Result<T>` veya `Result`) Controller'a dönmek. Kod akışını kontrol etmek (flow control) için "Exception" (istisna fırlatma) mekanizmasını kullanmaktan kaçınmak.
  - **Avantajı:** Exception fırlatmak sistem belleği ve performansı açısından maliyetlidir (stack trace oluşturulduğu için). Result Pattern ile iş mantığındaki (business logic) hatalar nesne tabanlı ve performanslı bir şekilde ele alınır. İstemciye (client) dönülecek HTTP Status kodları (404 Not Found, 400 Bad Request vb.) bu yapı sayesinde Controller'da çok daha temiz ve okunaklı bir şekilde kontrol edilir.

- **Repository Pattern** — Veri Erişim Soyutlaması
  - **Amaç:** Veritabanı (Data Access) işlemlerini merkezi bir yerde toplayarak servis katmanından soyutlamak.
  - **Avantajı:** Yarın farklı bir ORM'e (örneğin EF Core yerine Dapper) veya farklı bir veritabanına geçilmek istendiğinde projenin diğer katmanlarına dokunmadan sadece Repository sınıflarının güncellenmesi yeterli olur. Arayüzler (Interface) kullanıldığı için projeye bir Mock Data ekleyerek iş birimlerini test etmek inanılmaz derecede kolaylaşır.

- **Decorator Pattern** — Cross-Cutting İşlemleri Ekleme (Örn: Redis Cache)
  - **Amaç:** Kodun çekirdek yapısını (örneğin ana sorguyu) değiştirmeden, nesnenin üzerine dinamik olarak yeni bir sorumluluk (örneğin Cache kontrolü) sarmalamak.
  - **Avantajı:** "Open/Closed Principle"a %100 uyar. Kod değişime kapalı, gelişime açıktır. Ana veri çekme kodunun içerisine "Önce Redis'e bak, yoksa DB'den al" if-else mantığını bulaştırmamış oluruz.

- **Pipeline Behaviors** — Merkezi Çapraz Kesen Mantıklar (Cross-Cutting Concerns)
  - **Amaç:** MediatR araya girme mekanizmasını (Interceptor) kullanarak, istekler (requests) Handler'a ulaşmadan önce veya sonra çalışacak kurallar (Validation, Transaction, Logging) tanımlamak. `ICommand<T>` ve `IQuery<T>` ayrımlarının kullanılması.
  - **Avantajı:** Geliştiricinin her servis (Handler) içerisine tek tek try-catch yazması, transaction başlatıp commit/rollback yapması veya parametre doğrulama (Validation) kodları yazması engellenir. "Boilerplate" kodlardan kurtulunur. Özel arayüzler (`ICommand`) kullanıldığında TransactionBehavior sadece veri değiştiren komutlarda otomatik Transaction yönetimi sağlar.

- **Event-Driven Architecture** — RabbitMQ ile Gevşek Bağlılık (Loose Coupling)
  - **Amaç:** Mikroservislerin birbirleriyle doğrudan konuşması (Senkron HTTP Iletişimi) yerine, ortak bir kuyruk üzerinden asenkron event (olay) bazlı haberleşmesi.
  - **Avantajı:** Servislerin birbirine olan sıkı bağımlılığını (Tight Coupling) koparır. Bir servis geçici olarak çöktüğünde bile mesajlar kuyrukta bekler ve sistem ayağa kalktığında işlemi tamamlar (Resilience). Aynı event'i birden fazla servis dinleyebilir, bu da yeni modüllerin sisteme eklenmesini inanılmaz derecede kolaylaştırır.

## Mimari ve Tasarım Kararları (Architectural Decisions & Reasoning)

- **AutoMapper Yerine Manuel Mapper Kullanımı**
  - Sistem çok büyük çaplı olmadığı için ekstra bir bağımlılık getiren AutoMapper paketi yerine manuel mapper kullanmayı tercih ettim. Performans ve kontrol açısından daha şeffaf bir yapı sunmaktadır.
  - Ancak daha büyük ve çok fazla mapping işlemi barındıran projelerde geliştirme hızını artırmak için AutoMapper kullanımı önerilmektedir.

- **Hard Delete İşleminin Yalnızca ADMIN Tarafından Yapılabilmesi**
  - Sistemde satıcıların silmek istediği ürünler "Soft Delete" (pasife alma) yöntemiyle güncellenir. Bu yöntem, satıcının vazgeçmesi durumunda silinen ürünü kolayca tekrardan aktif edebilmesini sağlar.
  - Ürünlerin veritabanından kalıcı olarak (hard) silinmesi işlemi, veri güvenliği için sadece ADMIN rolü tarafından yapılabilir. İlerleyen süreçte 30 gün boyunca soft delete durumunda kalan ürünlerin gerçek silinme işlemi, ayrı bir `CleanUp` mikroservisi oluşturularak arka planda otomatikleştirilebilir.

- **ID Oluşturma Otomasyonu**
  - Veritabanı ID'leri, kod içerisinde manuel olarak set edilmek yerine, Entity (Varlık) constructor'ları içerisinde otomatik olarak oluşturulmaktadır. 
  - Bu sayede kod bütünlüğü korunur, geliştirici kaynaklı kimlik oluşturma unutkanlıkları ortadan kalkar ve yaratım kuralları Domain nesnesine encapsulate olur.

- **Dependency Injection (DI) Yönetimi**
  - Kod boyunca Primary Constructors yerine standart constructor injection kullanımı tercih edilmiş olup, DI servisleri private readonly ve alt çizgi (`_`) ön ekiyle tanımlanmıştır (ör. `_userRepository`).
  - Bu standartlaşma, kod tabanı üzerinde tutarlılık sağlar ve Dependency Injection mekanizmasının daha okunaklı ve sürdürülebilir bir yapıda kalmasına yardımcı olur.

- **Asenkron İptal Mekanizmaları - CancellationToken (CT)**
  - Tüm asenkron API endpoint'lerinden başlayarak Repository seviyesindeki (veri tabanı işlemleri) fonksiyonlara kadar `CancellationToken` aktarımı eksiksiz yapılmıştır.
  - İstemciler işlemlerini yarıda kestiğinde, CancellationToken sayesinde gereksiz veritabanı yorması engellenir, thread ve kaynak (memory) tasarrufu sağlanır.

- **Refresh Token Yenileme Sistemi**
  - Kullanıcının JWT (Access Token) yetki süresi dolduğunda veya token geçersiz kaldığında, ön yüzden otomatik olarak `refresh` endpoint'i çağrılır. Bu endpoint yetki doğrulaması (auth) gerektirmediği için sorunsuz çalışır.
  - Güvenlik sebebiyle HttpOnly Cookie içerisinde saklanan Refresh Token kullanılarak kullanıcının oturumu kapanmadan arka planda yeni bir Refresh Token ve JWT üretilir. Ardından bu yeni token'lar tekrar cookie'ye kaydedilerek kullanıcıya kesintisiz ve güvenli bir erişim sağlanır.

- **Log Servisinin Basit (Yalın) Tutulması**
  - Log servisinde DTO'lar, Pipeline Behavior'lar veya Unit of Work (UoW) gibi yapısal katmanlar kasten kullanılmadı ve veri loglanması doğrudan log dinleyen "Consumer" sınıfı içerisinde `SaveChanges` ile yapıldı.
  - Bu yöntemin izlenme nedeni: Log servisinin kompleks bir business logic barındırmaması ve çok daha az karmaşık olmasıdır. Bu sayede gereksiz mimari soyutlamalarından (over-engineering) kaçınılmış, servis oldukça hızlandırılmış ve basitleştirilmiştir.

- **Centralized Gateway ve Rate Limiting (İstek Sınırlandırma)**
  - Gateway kullanılarak mikroservisler için merkezi bir giriş kapısı tasarlandı. Artık dış dünyadan doğrudan servislere erişim sağlanamamakta, tüm istekler gateway kapısından incelenerek servislere ulaşmaktadır.
  - Sistem kaynaklarını tüketebilecek kötü niyetli veya tekrarlayan isteklere (DDoS vs.) karşı **Rate Limiting** yapılandırıldı. Genel API tüketimi için dakikada maksimum 80 istek kuralı uygulanırken, Auth (giriş, kayıt gibi) işlemleri özelinde brute-force denemelerini engellemek amacıyla dakikada maksimum 10 istek gibi daha kısıtlayıcı bir güvenlik politikası ayarlandı.

## API Endpoints

> 🚀 **Base URL İskeleti:** Tüm dış istekler Gateway üzerinden geçmelidir. Gateway varsayılan olarak `http://localhost:5292` portunda dinlemektedir.  
> **İstek Yapısı:** `http://localhost:5292/api/{controller}/{endpoint}/{parametreler}`
> 
> **Örnek İstek (Id ile ürün getir):** `GET https://localhost:5292//api/product/{id}`

### Auth Service
| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| POST | /api/auth/register | Kullanıcı kaydı | Hayır |
| POST | /api/auth/login | Giriş, token üret | Hayır |
| POST | /api/auth/refresh | Access token yenile | Hayır |

### Product Service
| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| GET | /api/product/ | Tüm ürün detayları | Hayır |
| GET | /api/product/{id} | Ürün detayı | Hayır |
| GET | /api/product/name/{product_name} | Ürün detayı | Hayır |
| POST | /api/product | Ürün ekle | Evet |
| PUT | /api/product/{id} | Ürün güncelle | Evet |
| DELETE | /api/product/hard-delete/{id} | Ürün sil | Evet |
| DELETE | /api/product/soft-delete/{id} | Ürün sil | Evet |

### Log Service
| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| GET | /api/log/logs | Tüm logları listele | Hayır |
| GET | /api/log/logs/info | Info (bilgi) seviyesindeki logları listele | Hayır |
| GET | /api/log/logs/error | Error (hata) seviyesindeki logları listele | Hayır |

## Environment Variables

| Değişken | Açıklama | Örnek |
|----------|----------|-------|
| Jwt__Secret | JWT imzalama anahtarı | min 32 karakter |
| Jwt__Issuer | Token üretici | AuthService |
| Jwt__Audience | Token hedef kitlesi | MiniCommerce |
| ConnectionStrings__Default | SQL Server bağlantısı | Server=... |
| ConnectionStrings__Redis | Redis bağlantısı | localhost:6379 |
| RabbitMQ__Host | RabbitMQ host | localhost |