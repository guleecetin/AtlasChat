# AtlasChat - .NET 8 & Ollama IVR Case Study

Bu proje, mülakat çalışması kapsamında **.NET 8 Web API** ve lokalde çalışan **Ollama (Llama 3)** entegrasyonu ile basit bir yapay zeka chat API'si olarak geliştirilmiştir. Ayrıca projenin Asterisk/IVR sistemleri ile mimari uyumu hedeflenmiştir.

---

## 🏗️ Proje Yapısı

* **Interface & DI:** Bağımlılıkları esnek tutmak için `IAIService` arayüzü ve constructor injection kullandım.
* **Resilience (Fallback):** Yapay zeka modeline ulaşılamazsa sistemin çökmemesi için kod içine akıllı bir fallback (yedek cevap) mekanizması ekledim.

```text
AtlasChat/
├── Controllers/
│   └── ChatController.cs     # API Endpoint (POST /api/chat)
├── Models/
│   └── ChatModels.cs         # İstek ve Cevap Modelleri (DTO)
├── Services/
│   ├── AIService.cs          # Ollama API Entegrasyonu ve Fallback Mantığı
├── Program.cs                # DI Kayıtları ve Uygulama Başlangıcı
└── appsettings.json          # Port (5000) ve Log Ayarları


#Kurulum ve Çalıştırma

#Ön Gereksinimler

Projeyi çalıştırmadan önce aşağıdaki bileşenlerin kurulu olmalıdır:

* .NET 8 SDK
* Ollama
* Llama 3 Modeli

---

## Yapay Zeka Servisinin Başlatılması

Öncelikle Ollama kurulumu tamamlandıktan sonra gerekli modeli indirip servisi başlatın:


ollama pull llama3
ollama serve


Bu adım sonrasında yapay zeka servisi lokal ortamda çalışmaya başlayacaktır.



## Projenin Çalıştırılması

Proje dizinine geçerek uygulamayı başlatın:


cd AtlasChat
dotnet run

## API Testi

Servisin doğru çalıştığını doğrulamak için aşağıdaki cURL komutu kullanılabilir:

curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d "{\"message\": \"Merhaba\"}"


Başarılı bir istekte API, JSON formatında yapay zeka cevabı döndürecektir.


# Teknik Değerlendirme Sorularının Cevapları

## 1️⃣ Asterisk Entegrasyonu

### Çağrı Nasıl Karşılanır?

Gelen çağrılar Asterisk'in `extensions.conf` dosyasında tanımlanan dialplan üzerinden karşılanır. Çağrı akışının yönetimi için AGI (Asterisk Gateway Interface) veya ARI (Asterisk REST Interface) mekanizmaları kullanılabilir.

### API Nasıl Çağrılır?

Kullanıcının konuşması STT (Speech-to-Text) servisi ile metne dönüştürüldükten sonra elde edilen veri:

* Asterisk içerisindeki `CURL()` fonksiyonu ile,
* veya arka planda çalışan bir servis (Python, Node.js vb.) aracılığıyla

`.NET Chat API` endpointine (`/api/chat`) POST isteği olarak gönderilir.

### Ses Nasıl Oynatılır?

API tarafından dönen metin yanıtı bir TTS (Text-to-Speech) servisi kullanılarak ses dosyasına dönüştürülür.

Oluşturulan ses dosyası Asterisk tarafında:

```asterisk
Playback(dosya_adi)


komutu ile arayan kullanıcıya dinletilir.



## 2️⃣ STT (Speech-to-Text) Tercihi

### Tercih Edilen Teknolojiler

* OpenAI Whisper (Lokal)
* Azure Speech Services

### Tercih Sebebi

Telefon görüşmelerinde karşılaşılan düşük ses kalitesi ve çevresel gürültüye rağmen Whisper yüksek doğruluk oranı sunmaktadır. Veri güvenliği gereksinimi olan kurumsal yapılarda ise lokal çalışan Whisper.cpp çözümü avantaj sağlamaktadır.



## 3️⃣ TTS (Text-to-Speech) Tercihi

### Tercih Edilen Teknoloji

* Azure Neural TTS

### Tercih Sebebi

Klasik TTS sistemleri robotik bir ses deneyimi oluşturabilmektedir. Azure Neural TTS, Türkçe dil desteğinde daha doğal vurgu ve tonlama sağlayarak kullanıcı deneyimini iyileştirmektedir.



## 4️⃣ Yapay Zeka Çözümü Tercihi

### Tercih Edilen Teknoloji

* Ollama
* Llama 3 (8B)

### Tercih Sebebi

* Tamamen lokal çalışabilmektedir.
* Veri gizliliğini korur.
* Ek bulut maliyeti oluşturmaz.
* Kurum içi kullanım senaryolarına uygundur.
* Servis kesintilerine karşı uygulama içerisinde fallback mekanizması uygulanmıştır.



## 5️⃣ Test Süreci

| Test Aşaması     | Kullanılan Araçlar      | Doğrulama Yöntemi                                                                                                             |
| ---------------- | ----------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| API Testi        | Postman / cURL          | `/api/chat` endpoint'ine istek gönderilerek `200 OK` ve doğru JSON yanıtı doğrulanır.                                         |
| Yapay Zeka Testi | Ollama CLI              | `ollama run llama3` komutu ile modelin yanıt verme süresi ve doğruluğu kontrol edilir.                                        |
| Asterisk Testi   | MicroSIP / Asterisk CLI | Softphone üzerinden çağrı gerçekleştirilir, `asterisk -rvvv` konsolundan API çağrıları ve ses oynatma işlemleri takip edilir. |

---

# 📌 Mimari Akış

Kullanıcı Araması
        │
        ▼
     Asterisk
        │
        ▼
 Speech-to-Text
        │
        ▼
   .NET Chat API
        │
        ▼
 Ollama (Llama 3)
        │
        ▼
 Text-to-Speech
        │
        ▼
     Playback
        │
        ▼
    Kullanıcı

