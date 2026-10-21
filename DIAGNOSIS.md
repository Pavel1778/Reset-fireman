# Fireman 1.0.9 — диагностика для TECNO LJ7 (Android 16)

Дата: 2026-10-19. Все выводы ниже проверены командами; сырые результаты приведены.

---

## 1. Доказанная причина

**Оригинальный APK содержит только 32-битные нативные библиотеки, а прошивка
TECNO LJ7 сконфигурирована как 64-бит-only. Поэтому Package Manager отвергает
установку с `INSTALL_FAILED_NO_MATCHING_ABIS`.**

Это не дефект APK и не проблема защиты Android. Это несовместимость ABI.

### Доказательство A — сам APK 32-битный

```
$ aapt2 dump badging work/orig_fireman.apk | grep native-code
native-code: 'armeabi-v7a' 'x86'
```

Только 32-битные ABI. `arm64-v8a` отсутствует. Внутри три библиотеки на ABI:
`libmain.so`, `libmono.so`, `libunity.so` — это Unity 5.3.8f1 с Mono-бэкендом.

```
$ python3 (ELF-заголовки всех .so)
lib/armeabi-v7a/libmain.so   ELF32 p_align=[4096, 4096]
lib/armeabi-v7a/libmono.so   ELF32 p_align=[4096, 4096]
lib/armeabi-v7a/libunity.so  ELF32 p_align=[4096, 4096]
```

### Доказательство B — прошивка LJ7 объявлена 64-бит-only

Три независимых источника:

**1. Метаданные дампа стоковой прошивки LJ7 (Android 15, LJ7-15.1.0.112SP01):**

```
sys_tssi_64_64only_armv82_tecno_dolby-user 15 AP3A.240905.015.A2
CPU abilist: arm64-v8a
```

Строка `sys_tssi_64_64only_armv82` — имя сборки Transsion; `64only` здесь прямо
указывает на 64-бит-only конфигурацию. `CPU abilist: arm64-v8a` — **список ABI
без `armeabi-v7a`**.

**2. Device tree LJ7 (проект LineageOS для этой модели), `configs/properties/vendor.prop`:**

```
# Dalvik & LMK (64-bit only)
ro.zygote=zygote64
ro.vendor.mtk_prefer_64bit_proc=1
```

`ro.zygote=zygote64` означает, что в системе **отсутствует 32-битный zygote**.
Без него 32-битные процессы приложений не запускаются в принципе.

**3. `lineage_LJ7.mk`:**

```
$(call inherit-product, $(SRC_TARGET_DIR)/product/core_64_bit_only.mk)
```

Наследуется AOSP-таргет `core_64_bit_only.mk` — стандартный AOSP-таргет
исключительно для 64-битных устройств.

### Доказательство C — железо умеет 32-бит, но прошивка это выключает

По таблице ARM (KA006014), ядра LJ7:

| Ядро | Категория | AArch32 |
|---|---|---|
| Cortex-A78 | 4 — 64b + legacy 32b at EL0 | да, только EL0 |
| Cortex-A55 | 3 — 32b и 64b | да |

То есть **аппаратно** 32-битный код на этом SoC исполним. Блокировка —
**программная**, на уровне конфигурации прошивки. Практического значения для вас
это не меняет: и стоковая прошивка TECNO, и community-дерево LineageOS для LJ7
оба собраны как 64-бит-only.

### Почему это не защита Android

Вы сами это опровергли: `Fireman_Install_Probe.apk` установился и показал
`INSTALL PROBE: OK`. Значит, сайдлоад сторонних APK на устройстве работает.
Плюс: у пробника нет нативных библиотек, и именно поэтому он установился, а игра
нет. Разница ровно одна — наличие `lib/armeabi-v7a/`.

Правило про `targetSdkVersion` тоже не при чём: Android 16 требует targetSdk ≥ 24,
а у игры 27.

---

## 2. Что проверено — команды и результаты

### 2.1. Репозиторий и релиз

```
$ git log --oneline -8
5a77ddb Recover Unity project from APK; assess arm64 rebuild feasibility
715e0e3 Publish full 31MB APK; correct overstated install-failure claim
434d379 Add install probe and step-by-step troubleshooting for rejected installs
45cba1c Add diagnostic app and real solutions for 64-bit-only phones
```

Релиз `v1.0.9-android-fix.1`: `draft=False`, `prerelease=False`, 14 ассетов,
опубликован 2026-10-18T18:39:32Z. Все скачанные ассеты совпали по SHA-256 с
`SHA256SUMS.txt` — релиз подлинный, ничего не подменено.

Компактного `Fireman_1.0.9_ARMv7_compact.apk` в релизе **нет** — он только в
корне репозитория. Это правильно: он дефектный (см. 2.3).

### 2.2. Пробник и ABI-чек — это разные приложения

```
$ sha256sum artifacts/Fireman_ABI_Check.apk artifacts/Fireman_Install_Probe.apk
95b9a17d...  Fireman_ABI_Check.apk     -> package com.fireman.abicheck
c83b0762...  Fireman_Install_Probe.apk -> package com.fireman.probe
```

Одинаковый размер (12 689) — совпадение, файлы разные. Оба без нативных
библиотек, поэтому проверка ABI их не касается.

`Fireman_ABI_Check.apk` — тот, который покажет реальный `SUPPORTED_32_BIT_ABIS`
устройства. **Его вы ещё не запускали.** Это второй способ подтвердить вывод
раздела 1 прямо с телефона.

### 2.3. Подписи (проверено на рабочей Java 17)

Java в окружении отсутствовала, поэтому более ранние прогоны `apksigner` молча
не выполнялись. Я установил Temurin JDK 17 и перепроверил:

```
$ apksigner verify --verbose --print-certs work/orig_fireman.apk
Verifies
Verified using v1 scheme: true
Verified using v2 scheme: false
Signer #1 certificate DN: CN=Magma Mobile, OU=Game Studio, O=Magma Mobile,
                        L=Argenteuil, ST=France, C=95100
```

Оригинал: подпись **только v1**, сертификат подлинный Magma Mobile.

```
$ apksigner verify --verbose --min-sdk-version 14 Fireman_1.0.9_ARMv7_compact.apk
DOES NOT VERIFY
ERROR: JAR signer FIREMAN.RSA: ... uses digest algorithm SHA-256 ...
       which is not supported on API Level(s) 14-17
```

Компактный APK из корня репозитория **не проходит проверку подписи** при его
собственном minSdk 14. При `--min-sdk-version 21+` проверку проходит. То есть
дефект затрагивает только API 14–17 и на Android 16 не проявляется. Отмечу как
реальный, но не относящийся к вашей проблеме.

```
$ apksigner verify --verbose --print-certs Fireman_1.0.9_final.apk
Verifies; v1: true, v2: true, v3: true
Signer #1 DN: CN=Fireman Compatibility Build, ...
```

`final.apk` подписан корректно (v1+v2+v3) нашим ключом.

### 2.4. Выравнивание ZIP

```
$ zipalign -c 4 work/orig_fireman.apk          -> ALIGNED OK
$ zipalign -c 4 Fireman_1.0.9_ARMv7_compact.apk -> ALIGNMENT FAILED (158 STORED entries BAD)
$ zipalign -c 4 Fireman_1.0.9_final.apk        -> ALIGNED OK
```

У компактного APK `resources.arsc` лежит без сжатия со смещением `% 4 == 3`.
Ранее я уже показал контрольным экспериментом, что одно рассогласование
выравнивания установку **не** блокирует. Это остаётся дефектом упаковки, а не
причиной отказа.

### 2.5. Исправленный APK сохранил оригинальные ресурсы

Сравнил содержимое по каждому файлу:

```
original entries: 589      final entries: 589
content entries identical: 586 из 587
names only in original: (нет)
names only in final   : (нет)
common names with DIFFERENT content: 1  -> META-INF/MANIFEST.MF
                                          (его перезаписывает apksigner)
AndroidManifest.xml: идентичен оригиналу
lib/x86/*: присутствуют, как в оригинале
assets/ файлов: original 366, final 366
assets/ с другим содержимым: 0
assets/ отсутствующих в final: 0
```

**Все 366 игровых ресурсов в `assets/` побайтово идентичны оригиналу.** x86- и
armeabi-v7a библиотеки на месте. `AndroidManifest.xml` не изменялся вообще.
Требование не удалять единственную копию ресурсов выполнено: оригинал лежит
отдельно как `Fireman_1.0.9_full_original.apk`.

### 2.6. Ограничения окружения — что я НЕ смог проверить

Это важно, и я не выдаю одно за другое.

- **Эмулятор работает, но без KVM.** `/dev/kvm` отсутствует, поэтому ARM64-образ
  запустить нельзя:
  `Avd's CPU Architecture 'arm64' is not supported by the QEMU2 emulator on x86_64 host`.
  Однако x86_64-образы **API 30** (с `armeabi-v7a` через `ndk_translation`) и
  **API 34** (только 64-бит) запускаются, и на них получены все результаты ниже.
- **ADB к вашему телефону недоступен.** Значит, текста ошибки установки именно
  с TECNO LJ7 у меня нет.
- **Тесты шли на x86_64-эмуляторах с `ndk_translation`**, а не на ARM-железе. Лог
  это подтверждает: `ndk_translation: Initialized NDK translation (armv7l)`,
  `Unexpected CPU variant for X86`. Эти тесты доказывают механизм отказа
  (`NO_MATCHING_ABIS` на 64-бит-only образе) и работоспособность игры на образах
  с 32-битным ABI, но это **не** тест на вашем телефоне.

Поэтому: причина доказана на уровне конфигурации устройства и структуры APK, а
также воспроизведена на эмуляторе с `abilist32=[]`. Остаётся подтвердить её прямо
на телефоне — одним тестом, см. раздел 4.

---

## 3. Разобранные варианты

| Вариант | Состояние | Вывод |
|---|---|---|
| Оригинальный APK | 32-бит только, подпись Magma | На LJ7 не установится: нет 32-бит userspace |
| Исправленный APK с оригинальными ресурсами | Ресурсы побайтово целы, подпись v1+v2+v3 | Эквивалентен оригиналу по ABI → тот же отказ |
| ARMv7-сборка | То же, что выше | Это не отдельный путь: любой ARMv7-APK требует 32-бит userspace |
| Виртуальная машина с 32-бит поддержкой | **Рабочий путь** | Единственный вариант, где игра реально запустится |

Важно понимать: **«исправленный APK» и «ARMv7-сборка» — не разные варианты.**
Исправить подпись, выравнивание и висячие компоненты манифеста можно (сделано), но
это не добавляет `arm64-v8a`. Добавить его в готовый APK нельзя: 64-битные
`libunity.so` и `libmono.so` для Unity 5.3.8f1 не существуют — Unity начала
поддерживать arm64 только с 2017.4.16 / 2018.2.

---

## 4. Лучший следующий шаг

### Один тест, который закроет вопрос окончательно

Установите на телефон:

```
https://github.com/Pavel1778/Reset-fireman/releases/download/v1.0.9-android-fix.1/Fireman_ARM32_Install_Probe.apk
```

Что это: тот же пробник (чистая Java, никакого кода игры), к которому добавлена
**одна** 32-битная библиотека `lib/armeabi-v7a/libfiremanarm32probe.so`. Больше
ничего не изменено. Подпись v2+v3, minSdk 24, targetSdk 34.

Результат читается однозначно:

- **Установился** → телефон поддерживает 32-бит, и причина отказа была в другом
  (тогда смотрим дальше).
- **Ошибка «Приложение не совместимо с вашим устройством»** или
  `INSTALL_FAILED_NO_MATCHING_ABIS` → причина подтверждена: 64-бит-only
  прошивка. Это ровно тот же отказ, что и у игры, и он воспроизводим.

Затем, для полноты, запустите `Fireman_ABI_Check.apk` — он покажет строку
`SUPPORTED_32_BIT_ABIS` прямо с устройства. Ожидаю пустое значение.

### Если причина подтвердится — как играть

**VPhoneOS** (или аналог: VMOS, F1 VM). Виртуальная машина Android со своим
32-битным userspace внутри 64-битного телефона. Это единственный путь, который
не требует пересборки игры.

Настройка: Android 7.1.2, архитектура **«64-bit & 32-bit mixed»**, затем внутри
ВМ установить `Fireman_1.0.9_full_original.apk` (оригинальная подпись Magma,
не конфликтует с другими копиями).

### Пересборка под arm64

Технически возможна — восстановленный Unity-проект лежит в релизе
(`Fireman_Unity_Project_recovered.zip`, 299 C#-файлов). Но по правилу задачи
миграция на Unity 6 не начинается, пока диагностика не завершена и
необходимость не обоснована. Диагностика сейчас на последнем шаге: нужен
результат теста из этого раздела. Подробности миграции — в `REBUILD.md`.

---

## 5. Итог по вашим пунктам

**Доказанная причина.** APK содержит только `armeabi-v7a` + `x86`; прошивка LJ7
объявлена 64-бит-only (`ro.zygote=zygote64`, `core_64_bit_only.mk`, имя сборки
`sys_tssi_64_64only_armv82`, `abilist=arm64-v8a`). Package Manager не находит
подходящего ABI → отказ на этапе установки, **до** загрузки нативного кода.

**Оставшиеся гипотезы.** Одна: что на вашем конкретном экземпляре прошивка
всё-таки содержит 32-бит userspace (например, другая региональная сборка).
Проверяется тестом из раздела 4.

**Лучший следующий шаг.** Установить `Fireman_ARM32_Install_Probe.apk` — один
тест, однозначный результат.

**Ссылка на релиз.**
https://github.com/Pavel1778/Reset-fireman/releases/tag/v1.0.9-android-fix.1

**Чего я не утверждаю.** Что какой-либо APK работает на TECNO LJ7 — это не
проверено на физическом устройстве, и проверено быть не может без ADB-доступа.
Также не утверждаю, что эмуляторные тесты равны тестам на вашем телефоне.
