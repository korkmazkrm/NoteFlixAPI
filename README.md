# NoteFlix Backend API

ASP.NET Core Web API projesi - Kullanıcı authentication ve premium özellik kontrolü için.

## Özellikler

- JWT tabanlı authentication
- User registration ve login
- Premium status kontrolü
- Refresh token desteği
- SQL Server veritabanı

## Kurulum

1. **Veritabanı Kurulumu**
   ```bash
   # Entity Framework migration'ları oluştur
   dotnet ef migrations add InitialCreate
   
   # Veritabanını oluştur
   dotnet ef database update
   ```

2. **Uygulama Ayarları**
   - `appsettings.json` dosyasında connection string'i güncelle
   - JWT secret key'i değiştir

3. **Uygulamayı Çalıştır**
   ```bash
   dotnet run
   ```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Kullanıcı kaydı
- `POST /api/auth/login` - Kullanıcı girişi
- `POST /api/auth/refresh-token` - Token yenileme
- `POST /api/auth/logout` - Çıkış yapma

### User
- `GET /api/user/premium-status` - Premium durum kontrolü
- `GET /api/user/profile` - Kullanıcı profili

## Güvenlik

- JWT token'ları 1 saat geçerli
- Refresh token'ları 7 gün geçerli
- BCrypt ile şifre hash'leme
- CORS policy yapılandırılmış

## Veritabanı

- **Users**: Kullanıcı bilgileri
- **UserSessions**: Refresh token'ları

## Notlar

- Production'da JWT secret key'i güvenli bir şekilde sakla
- Connection string'i environment variable olarak ayarla
- HTTPS kullan
