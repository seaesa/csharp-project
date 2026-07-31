# FarmNet - Tong hop luong he thong, blockchain, use case va nghiep vu

## 1) Muc tieu he thong

FarmNet la he thong quan ly lo san pham nong nghiep, theo doi toan bo qua trinh canh tac va cung cap truy xuat nguon goc co doi chieu blockchain.

Muc tieu chinh:
- Quan ly lo san pham theo trang trai.
- Ghi nhan nhat ky canh tac va du lieu IoT theo thoi gian.
- Tao dau vet toan ven du lieu bang hash SHA-256 theo ngay.
- Dong bo hash len blockchain de tang tinh minh bach.
- Cung cap endpoint public de nguoi dung cuoi truy xuat va xac minh.

---

## 2) Kien truc tong the (nhin tu nghiep vu)

### Thanh phan
- `FarmNet.Api`: Cac REST API cho Backoffice (co xac thuc) va Public Trace.
- `FarmNet.Infrastructure`: Service xu ly nghiep vu, persistence, blockchain integration.
- `FarmNet.Domain`: Entity, enum, interface.
- SQL Server: Luu du lieu nghiep vu va metadata blockchain.
- Blockchain node (Ganache local trong moi truong dev): Luu giao dich chua hash.

### Kieu du lieu chinh
- `Batch`: Lo san pham.
- `FarmingLog`: Nhat ky cong viec canh tac.
- `SensorData`: Du lieu cam bien/IoT.
- `DailyHash`: Hash tong hop theo ngay cho 1 lo.
- `Harvest`: Thong tin thu hoach.
- `BlockchainRecord`: Ban ghi su kien da/chu a ghi len blockchain (TaoLo, ThuHoach, ...).

---

## 3) Luong he thong end-to-end

### Luong A - Tao lo san pham
1. User (Admin/FarmOwner) goi `POST /api/batches`.
2. He thong tao `Batch` trong DB.
3. He thong tao `dataHash` tu thong tin lo (id, ma lo, farm, ngay tao).
4. Goi blockchain service de ghi hash len chain.
5. Luu `BlockchainRecord` loai `TaoLo` trong DB (kem `TxHash`, `DaXacNhan`).

Gia tri dat duoc:
- Ngay tu luc tao lo da co dau vet blockchain.

### Luong B - Ghi nhat ky canh tac
1. User (Admin/FarmOwner/Worker) goi `POST /api/farming-logs`.
2. He thong luu `FarmingLog`.
3. He thong recalc `DailyHash` cho ngay co log vua ghi.
4. `DailyHash` duoc cap nhat hash moi va danh dau chua xac nhan.

### Luong C - Ghi du lieu IoT
1. Thiet bi goi `POST /api/sensors/data` (anonymous).
2. API parse payload IoT (`device`, `batchID`, `time`, `temp`, `hum`, `rain`, `water`, `gas`, `pump`).
3. Mapping ve `SensorData` (vi du `rain=0` -> co mua, `pump!=0` -> bom bat).
4. Luu `SensorData` vao DB.
5. Recalc `DailyHash` theo ngay cua ban ghi IoT.

### Luong D - Commit hash ngay len blockchain
1. User (Admin/FarmOwner) goi `POST /api/batches/{id}/commit-blockchain`.
2. He thong recalc lai toan bo ngay co du lieu cho lo.
3. Lay cac `DailyHash` chua xac nhan (`DaXacNhan=false`).
4. Moi ngay tao 1 tx blockchain voi hash ngay.
5. Cap nhat `TxHash`, `DaXacNhan` cho tung ban ghi `DailyHash`.

Luu y:
- Message API hien tai ghi "commit merkle root", nhung implementation dang ghi hash theo tung ngay, chua tao cay merkle.

### Luong E - Thu hoach
1. User (Admin/FarmOwner) goi `POST /api/harvests`.
2. Luu `Harvest`, cap nhat trang thai lo thanh `DaThuHoach`.
3. Tao hash su kien thu hoach va ghi len blockchain.
4. Luu `BlockchainRecord` loai `ThuHoach`.
5. Tu dong goi commit daily hash cho lo (dam bao chuoi du lieu duoc "dong" khi ket thuc canh tac).

### Luong F - Truy xuat cong khai
1. Nguoi dung cuoi quet ma/nhap ma lo -> goi `GET /api/public/trace/{batchMaLo}`.
2. He thong tra ve:
   - Thong tin lo + farm.
   - Nhat ky canh tac.
   - Thu hoach (neu co).
   - Du lieu cam bien gan nhat.
   - Danh sach blockchain records.
3. Neu can xac minh toan ven: goi `GET /api/public/trace/{batchMaLo}/verify`.

---

## 4) Co che blockchain va toan ven du lieu

## 4.1 Kieu hash
- Dung SHA-256, output hex lowercase.
- Hash duoc tao tu chuoi canonical (co thu tu, co timestamp UTC theo ISO 8601 voi `:O`).

### 4.2 Su kien duoc ghi len chain
- `TaoLo`: hash thong tin tao lo.
- `ThuHoach`: hash thong tin thu hoach.
- `TongHopHangNgay`: hash tong hop toan bo log + sensor cua 1 ngay.

### 4.3 Cau truc payload tx
Service blockchain ghi `input data` dang text:
- `FarmNet|{EventType}|{BatchRef}|{DataHash}|{TimestampUtc}`

Tx duoc gui den chinh dia chi account tren node Ethereum (muc tieu la neo du lieu hash vao blockchain, khong can smart contract).

### 4.4 Cach verify
Voi moi muc can verify:
1. Recompute hash tu DB hien tai.
2. So sanh voi hash dang luu trong DB (`DailyHash`/`BlockchainRecord`).
3. Neu co `TxHash`, doc lai tx tren blockchain:
   - Tach `DataHash` tu payload tx.
   - So sanh voi hash DB.

Trang thai verify:
- `HopLe`: khop DB va khop blockchain.
- `ChuaXacNhan`: chua co tx hoac tx khong tim thay.
- `BiThayDoi`: du lieu DB da thay doi so voi hash da luu.
- `BlockchainKhongKhop`: hash trong tx khong giong hash DB.
- `KhongTruyCapBlockchain`: loi RPC / khong truy cap duoc node.

### 4.5 Co che phat hien sua doi sau commit
- Khi log/sensor thay doi, daily hash duoc recalc.
- Neu ban ghi da tung co `TxHash`, he thong giu lai `TxHash`.
- Luc verify se thay hash moi (DB) khong khop hash cu tren chain -> phat hien thay doi.

---

## 5) Use case theo vai tro

### Admin
- Dang nhap he thong.
- Tao/sua/cap nhat trang thai lo.
- Tao/xoa cam bien.
- Ghi nhat ky.
- Tao thu hoach.
- Commit blockchain cho lo.
- Xem verify blockchain va daily hashes.
- Xem du lieu IoT gan day.

### FarmOwner
- Tuong tu Admin trong pham vi nghiep vu trang trai/lo.
- Tao lo, ghi nhat ky, thu hoach, commit blockchain.

### Worker
- Ghi nhat ky canh tac.
- Xem du lieu lien quan (theo quyen duoc cap).

### IoT Device (ESP32, ...)
- Day du lieu cam bien qua API public `POST /api/sensors/data`.

### Nguoi dung truy xuat (consumer, doi tac, co quan)
- Xem thong tin truy xuat qua API public.
- Xem ket qua verify toan ven du lieu.

---

## 6) Usage flow nghiep vu de van hanh thuc te

### Giai doan 1 - Khoi tao
1. Tao Farm.
2. Tao Batch (`MaLo` la dinh danh truy xuat).
3. (Tuy chon) dang ky sensor device cho lo.

### Giai doan 2 - Trong qua trinh canh tac
1. Cong nhan chu dong ghi `FarmingLog` theo cong viec.
2. IoT day du lieu dinh ky.
3. He thong tu dong cap nhat daily hash moi ngay.

### Giai doan 3 - Dong bo blockchain theo chu ky
1. Theo moc (cuoi ngay/cuoi tuan), chu lo goi commit.
2. He thong day cac ngay chua xac nhan len blockchain.
3. Theo doi `TxHash` va trang thai xac nhan.

### Giai doan 4 - Thu hoach va khoa chu ky
1. Tao ban ghi thu hoach.
2. He thong ghi hash thu hoach len chain.
3. He thong commit daily hash de "chot" bo du lieu canh tac.

### Giai doan 5 - Truy xuat va doi chieu
1. Nguoi dung quet ma lo.
2. Xem toan bo timeline lo.
3. Goi verify de xem ket qua toan ven tong quan va chi tiet theo ngay.

---

## 7) Danh sach endpoint chinh (tham chieu nhanh)

### Noi bo (co xac thuc)
- `POST /api/batches` - Tao lo.
- `GET /api/batches/{id}/blockchain` - Xem ban ghi blockchain.
- `GET /api/batches/{id}/verify` - Verify blockchain cho lo.
- `GET /api/batches/{id}/daily-hashes` - Xem hash theo ngay.
- `POST /api/batches/{id}/commit-blockchain` - Commit hash ngay len chain.
- `POST /api/farming-logs` - Ghi nhat ky canh tac.
- `POST /api/harvests` - Ghi nhan thu hoach.
- `POST /api/sensors/data` - IoT day du lieu (anonymous, nhung thuoc he thong noi bo).

### Public truy xuat
- `GET /api/public/trace/{batchMaLo}` - Lay thong tin truy xuat.
- `GET /api/public/trace/{batchMaLo}/verify` - Xac minh toan ven.

---

## 8) Luu y van hanh va gioi han hien tai

- Blockchain co che "best effort": neu chua cau hinh RPC/private key hop le, nghiep vu blockchain bi bo qua.
- Du lieu van luu DB binh thuong, nhung khong co tx hash de doi chieu.
- He thong hien tai neo hash vao tx input, chua dung smart contract.
- "Merkle root" trong message API chua phu hop implementation thuc te (dang la hash theo ngay).
- Verify dang sample theo so ngay (mac dinh 10) voi daily hash, kem verify 2 moc dau-cuoi (`TaoLo`, `ThuHoach` neu co).

---

## 9) De xuat nang cap tiep theo (neu can)

- Tao merkle tree thuc su tren danh sach hash ngay, ghi 1 merkle root/ky commit.
- Bo sung smart contract de luu event chuan hoa (de index va query tot hon).
- Co scheduler commit tu dong (hang ngay) thay vi thu cong.
- Bo sung co che signature theo thiet bi IoT de tang tinh tin cay payload dau vao.
- Co dashboard canh bao ngay nao bi `BlockchainKhongKhop` hoac `BiThayDoi`.

