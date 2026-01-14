
# VetClinicSystem API 

[![.NET Core CI](https://github.com/ertugrulsak/VetClinicSystem/actions/workflows/main.yml/badge.svg)](https://github.com/ertugrulsak/VetClinicSystem/actions/workflows/main.yml)
[![codecov](https://codecov.io/gh/ertugrulsak/VetClinicSystem/graph/badge.svg)](https://codecov.io/gh/ertugrulsak/VetClinicSystem)

## Proje Açıklaması

**VetClinicSystem**, veteriner kliniklerinin günlük operasyonlarını dijital ortamda yönetmelerini sağlayan kapsamlı bir **RESTful API** projesidir. Bu sistem; hasta kayıtları, randevu takibi, tedavi süreçleri, fiyatlandırma ve veteriner hekim yönetimi gibi temel fonksiyonları uçtan uca (End-to-End) kapsar.

Bu proje, **Yazılım Kalite ve Test** dersi kapsamında geliştirilmiş olup; **Birim (Unit)**, **Entegrasyon (Integration)** ve **Sistem (System)** testleri ile %90 üzerinde test kapsamına (coverage) sahiptir. Ayrıca CI/CD süreçleri ile tam otomasyona geçirilmiştir.

### Kullanılan Teknolojiler ve Araçlar
* **.NET 8.0 SDK** (Backend Framework)
* **Entity Framework Core** (ORM)
* **xUnit** (Test Framework)
* **SQLite** (Veritabanı)
* **GitHub Actions** (CI/CD Pipeline)
* **Codecov** (Code Coverage Reporting)
* **Swagger / OpenAPI** (API Dokümantasyonu)

---

## Kurulum Talimatları (Adım Adım)

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları takip edebilirsiniz:

### 1. Projeyi Klonlayın
```bash
git clone [https://github.com/ertugrulsak/VetClinicSystem.git](https://github.com/ertugrulsak/VetClinicSystem.git)
cd VetClinicSystem

```

### 2\. Bağımlılıkları Yükleyin

Proje bağımlılıklarını (NuGet paketleri) yüklemek için:

Bash

```
dotnet restore

```

### 3\. Veritabanını Oluşturun

Proje SQLite kullandığı için veritabanı dosyası otomatik oluşacaktır, ancak şemanın güncel olduğundan emin olmak için migration uygulayın:

Bash

```
cd VetClinicAPI
dotnet ef database update

```

### 4\. Uygulamayı Başlatın

Uygulamayı ayağa kaldırmak için:

Bash

```
dotnet run

```


* * * * *

API Endpoint'leri ve Kullanım
--------------------------------

API, aşağıdaki temel kaynaklar (Resources) üzerinde CRUD işlemlerine izin verir:

| **Metot** | **Endpoint** | **Açıklama** |
| --- | --- | --- |
| **GET** | `/api/PetOwners` | Tüm hayvan sahiplerini listeler. |
| **POST** | `/api/PetOwners` | Yeni bir hayvan sahibi kaydeder. |
| **GET** | `/api/Pets` | Kayıtlı tüm evcil hayvanları getirir. |
| **POST** | `/api/Pets` | Sisteme yeni bir evcil hayvan ekler (**Owner ID** gerektirir). |
| **GET** | `/api/Veterinarians` | Klinikteki veteriner hekimleri listeler. |
| **POST** | `/api/Appointments` | Yeni bir randevu oluşturur. |
| **POST** | `/api/Treatments` | Randevuya bağlı tedavi/işlem ekler. |


* * * * *

Swagger / OpenAPI Dokümantasyonu
-----------------------------------

Proje çalıştırıldığında tarayıcınızdan aşağıdaki adrese giderek interaktif API dokümantasyonuna erişebilirsiniz:

**Dokümantasyon URL:** [https://localhost:7175/swagger](https://www.google.com/search?q=https://localhost:7175/swagger)

> Alternatif Link: http://localhost:5079/swagger
>
> (Eğer SSL sertifikasıyla ilgili yerel bir sorun yaşarsanız HTTP linkini kullanabilirsiniz.)

* * * * *

 Testleri Çalıştırma
----------------------

Testleri çalıştırmak için ana dizinde şu komutları kullanın:

**Tüm Testleri Çalıştır:**

Bash

```
dotnet test

```

**Code Coverage Raporu Üret:**

Bash

```
dotnet test --collect:"XPlat Code Coverage"

```

### Test Stratejisi ve Kapsamı

1.  **Unit Tests:** Controller ve Business Logic katmanlarının izole testleri.

2.  **Integration Tests:** API Endpoint'leri ve Veritabanı etkileşimini doğrulayan testler.

3.  **System (E2E) Tests:** Kullanıcı senaryolarını (Örn: Kayıt -> Randevu -> Tedavi) simüle eden uçtan uca testler.

* * * * *

CI/CD ve Otomasyon
---------------------

Bu proje **GitHub Actions** kullanılarak sürekli entegrasyon sürecine dahil edilmiştir. `main` dalına yapılan her `push` ve `pull_request` işleminde:

1.  Proje sanal bir Linux ortamında derlenir.

2.  Tüm testler otomatik olarak koşulur.

3.  Test kapsamı (Coverage) hesaplanıp **Codecov** servisine raporlanır.

4.  Sonuçlar `README.md` dosyasındaki rozetlere (badges) yansıtılır.


----------------------
NOT: Codecov'da migrations'ları da dahil ettiği için yüzdelik dilimi düşük gözüküyor. Migrations hariç 292 satır kodun 192 satırını kapsıyor. Bu da yaklaşık %65'lik bir kapsamaya denk geliyor.
