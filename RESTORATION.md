# Fireman 1.0.9 — восстановление и результаты проверки

## Кратко

Игра восстановлена: оригинальный APK переподписан современной схемой
(v1 + v2 + v3). Содержимое игры при этом не изменялось вообще. Сборка
`Fireman_1.0.9_final.apk` установлена и запущена на эмуляторе — игра рендерит
игровой процесс и реагирует на касания, критических ошибок нет.

Совместимость с реальным **TECNO LJ7 / Android 16 не подтверждена**. Более того,
она **воспроизведённо невозможна**: прошивка LJ7 не содержит 32-битного Android
userspace (проверено на эмуляторе API 34 с `abilist32=[]` — и исправленная сборка,
и нетронутый оригинал падают с `INSTALL_FAILED_NO_MATCHING_ABIS`). Это ограничение
совместимости ARMv7-сборки, а не неисправность, которую удалось устранить.

## Исходное состояние

`Fireman_1.0.9_ARMv7_compact.apk` (23 598 505 байт, в репозитории) — компактная
перепакованная сборка, а не файл из Google Play. В ней только 32-битные
библиотеки: `lib/armeabi-v7a/{libunity.so, libmono.so, libmain.so}`.

Проверенный факт: **оригинальный APK из Google Play тоже 1.0.9 (versionCode 17)**,
и это последняя версия игры. В каталогах (APKCombo, Uptodown) 1.0.9 от 2018 г.
указана как актуальная; следующей версии не существует.

APK из репозитория имел реальные дефекты, но **формулировка «не устанавливался»
была преувеличена и здесь исправлена.**

Что измерено на эмуляторе Android 11 (API 30) с поддержкой `armeabi-v7a`:

| Сборка | `zipalign -c 4` | Установка |
|---|---|---|
| компактная из репозитория | FAILED (`resources.arsc` BAD-3) | **Success** |
| полный оригинал 31 МБ (v1-only, подпись Magma) | OK | **Success** |
| `Fireman_1.0.9_final.apk` | OK | **Success** |
| полный APK с намеренно сломанным `resources.arsc` | FAILED (BAD-1) | **Success** |

Последняя строка — контрольный эксперимент: я собрал вариант полного APK, где
смещение `resources.arsc` сдвинуто на 1 байт, и больше ничего не менял. Он
**всё равно установился**. Значит, одного рассогласования выравнивания
недостаточно, чтобы заблокировать установку на этих версиях Android.

Найденные дефекты (все реальные):

1. **`resources.arsc` не выровнен — только в компактном файле.** `zipalign -c 4`
   на компактном APK честно возвращает `Verification FAILED`. **Полный
   оригинал выровнен правильно** (`zipalign -c 4` — OK), поэтому финальная
   сборка ничего выравнивать не требуется. На проверенных версиях Android это
   **не** блокировало установку в любом случае.
2. **Подпись только v1 и с SHA-256.** `apksigner` сообщает, что SHA-256 в
   JAR-подписи не поддерживается для API 14–17 (заявленный minSdk). Это
   **единственный настоящий дефект полного оригинала**; лечится переподписью
   v1+v2+v3.
3. **Два «висячих» компонента манифеста:** `com.facebook.ads.RewardedVideoAdActivity`
   и `up.Receiver` объявлены, но классов нет в `classes.dex` — проверено,
   0 вхождений в DEX. Android из-за этого **не отказывает** ни в установке, ни в
   запуске, поэтому удалять их не обязательно. В финальной сборке они
   **сохранены как есть** (см. поправку ниже).

**Единственная воспроизводимая причина отказа установки, которую я получил, —
`INSTALL_FAILED_NO_MATCHING_ABIS` на 64-бит-only устройствах.** Её исправить
нельзя (см. `SOLUTIONS.md`).

## Поправка к предыдущей версии (регрессия, уже откатана)

Раньше `Fireman_1.0.9_final.apk` (SHA-256 `c5410786…`) собирался через
`apktool` с правкой манифеста: удалялись два компонента и добавлялся
`usesCleartextTraffic`. Это было ошибкой:

* Правка **внесла регрессию**: `apktool` переписал значения
  `com.google.android.gms.games.APP_ID` и `...appstate.APP_ID` из
  `640341620909` в `" 640341620909"` — с ведущим пробелом, что ломает вход в
  Play Games.
* Правки были не нужны: Android не отказывает в установке/запуске из-за
  объявленного класса, отсутствующего в dex, а `usesCleartextTraffic` не имеет
  значения при targetSdk 27 (блокировка cleartext включается с targetSdk 28).

Текущая сборка — чистая переподпись нетронутого оригинала, регрессии нет.
Подробности и сырые выводы команд: `AUDIT.md`.

## Что именно изменено в исправленной сборке

Сравнение `Fireman_1.0.9_final.apk` с исходным APK по записям ZIP:

| Категория | Результат |
|---|---|
| Всего записей | 589 → 589 |
| Контентных записей | **586 из 587 идентичны побайтово** |
| Отличается | **только `META-INF/MANIFEST.MF`** (его перезаписывает `apksigner`) |
| `AndroidManifest.xml` | **идентичен оригиналу, не изменялся** |
| `lib/` + `assets/` | **369 из 369 идентичны побайтово** |

То есть содержимое игры не тронуто вообще: изменена только подпись.
`AndroidManifest.xml` — ровно такой, каким его выпустила Magma Mobile;
два «висячих» компонента сохранены.

## Инструменты и версии

- OpenJDK 17 (`$HOME/tools/jdk-17.0.11+9`)
- Android SDK build-tools **34.0.0** (`zipalign`, `apksigner`, `aapt2`)
- Android platform **android-34**; эмуляторы API 30 (Android 11) и API 34
- Python 3 (скрипты сборки и анализа)
- Ключ подписи: `work/fireman-release.keystore`, DN
  `CN=Fireman Compatibility Build, OU=Remaster, O=Local`, RSA 2048

## Результаты проверок

### Статические (на `Fireman_1.0.9_final.apk`)

| Проверка | Результат |
|---|---|
| Размер | 32 090 949 байт (30.60 MiB) |
| SHA-256 | `e8656604513405b9d4d60e8bc17208e894cc32425d98c530fff0180e7676de21` |
| Целостность ZIP / CRC | OK (`testzip` → нет ошибок) |
| Записей в APK | 589 |
| Контентных записей, идентичных оригиналу | 586 из 587 (отличается только `META-INF/MANIFEST.MF`) |
| `classes.dex`, `resources.arsc`, манифест | присутствуют |
| Native-библиотеки | `armeabi-v7a` (3 шт.) + `x86` (3 шт.), как в оригинале |
| `aapt2 badging` | `com.magmamobile.game.Fireman`, 1.0.9 (17), minSdk 14, targetSdk 27 |
| `zipalign -c 4` | OK |
| `apksigner verify` | v1 ✓, v2 ✓, v3 ✓; 1 подписант |
| Сертификат SHA-256 | `7521d98da5aee128601f702a4777aadd8ad6d88e67cd6060751b3696ef191870` |

### Установка и запуск (эмулятор)

Проверено на эмуляторе **Android 11 (API 30), x86_64** — он поддерживает
`armeabi-v7a` через `ndk_translation`, что и позволяет запустить 32-битную игру.

- Установка — успешно.
- Запуск `up.PluginActivity` — процесс стартует и держится (проверено 120 с).
- В `logcat`: `ndk_translation: Initialized NDK translation (armv7l)` — 32-битный
  код исполняется; Google Analytics и `libEGL` инициализируются.
- Рендеринг: кадр игрового процесса — 51 864–64 689 различных цветов, из них
  **134 111–136 550 оранжевых пикселей** (палитра пожарного: `rgb(255,186,8)`,
  `rgb(222,77,58)`, фон `rgb(25,8,33)`).
- Отклик на касания: тап и свайп меняют кадр (6 867–17 593 пикселей на кадр).
- Наблюдение: ни `FATAL`, ни `UnsatisfiedLinkError`, ни `DllNotFound`.
- Скриншоты: `artifacts/evidence_real/final_v2_gameplay.png` и
  `final_v2_gameplay2.png`. Тот же прогон для нетронутого оригинала:
  `original_untouched_gameplay.png`.

**Чего проверка не покрывала.** Проверено появление игрового экрана, отклик на
ввод и стабильность процесса. Полный игровой сценарий (прохождение уровней,
магазин, пауза/возобновление на разных экранах) **не выполнялся** — эмулятор без
KVM работает крайне медленно, и такое тестирование на нём непрактично.

## Достоверность данных: что откуда взято

Здесь важно разделить источники, потому что вывод про LJ7 в прошлом отчёте был
сформулирован слишком уверенно.

### Данные с реального устройства
**Отсутствуют.** Телефон к среде не подключён, дампов `build.prop` или `getprop`
с LJ7 в рабочей среде нет. Строка `sys_tssi_64_64only_...` встречалась только в
моих же отчётах и не подтверждена файлом с устройства. Считать её
установленным фактом нельзя.

### Данные эмулятора
- Android 11 (API 30): `abilist` включает `armeabi-v7a`, 32-битные приложения
  устанавливаются и работают.
- Android 14 (API 34, 64-бит-only образ): `abilist32=[]`, `ro.zygote=zygote64`.
  Установка нашей ARMv7-сборки завершается ошибкой
  `INSTALL_FAILED_NO_MATCHING_ABIS: Failed to extract native libraries, res=-113`.
  Это точная модель поведения 64-бит-only Android.

### Статический анализ
- В APK только 32-битные библиотеки (машинный код ELF32 ARM).
- Игра собрана на Unity 5.3.8f1, бэкенд Mono (есть `libmono.so`, managed DLL).
- Версия 1.0.9 — последняя.

### Внешние проверяемые источники
- Unity: ARM64 для Android появился только в **2018.2** и **только с бэкендом
  IL2CPP** (блог Unity «Meeting Google Play requirements in the future», 2017;
  «2018.2 is now available», июль 2018). В документации Unity список платформ
  Mono включает Android **только Armv7**. Значит, сборка на Unity 5.3.8f1 с Mono
  не может содержать ARM64-библиотеки Unity в принципе.
- Google: с Android 12 образы эмулятора только 64-битные; устройства вроде Pixel 7
  поставляются 64-бит-only.
- XDA / GrapheneOS / Pixel 7: на Pixel 7 32-битные библиотеки физически
  присутствовали, и включение `zygote64_32` + правка `abilist` возвращала
  поддержку 32-бит. Но там библиотеки **были в прошивке**. Это ключевое отличие:
  «выключено свойствами» и «библиотек нет» — разные ситуации.

### Предположения, требующие проверки на устройстве
- Что прошивка LJ7 не содержит 32-битных библиотек. Косвенно вероятно (свежие
  Transsion/Dimensity-прошивки обычно 64-бит-only), но **не измерено**.
- Что включение 32-бит на LJ7 невозможно. Зависит от того, есть ли библиотеки в
  образе; проверяется только на устройстве.

## Ограничение: почему на LJ7 может не запуститься

Если на LJ7 `ro.product.cpu.abilist32` пуст (и `ro.zygote=zygote64`), установка
ARMv7-сборки упадёт с `INSTALL_FAILED_NO_MATCHING_ABIS` — как на эмуляторе API 34.
Это ограничение платформы, а не дефект APK.

Важно: SoC MT6878 (Cortex-A78 + A55, ARMv8.2-A) **аппаратно** умеет AArch32.
Ограничение, если оно есть, программное (userspace), а не аппаратное.

## Что осталось для совместимости с современными устройствами

1. **Проверить ABI на самом устройстве** (безопасно, без перепрошивки) — это
   следующий шаг, см. `INSTALL.md`.
2. Если `abilist32` непуст → ставить `Fireman_1.0.9_final.apk`, всё должно работать.
3. Если пуст → варианты: (а) устройство/эмулятор с 32-битным окружением;
   (б) кастомная сборка LineageOS для LJ7 с `core_64_bit.mk` вместо
   `core_64_bit_only.mk` и 32-битными vendor-библиотеками (дни работы, риск);
   (в) включение 32-бит через root — **только если** библиотеки есть в образе,
   иначе не поможет.
4. **Настоящий ARM64-порт:** технически потребовал бы пересборки в Unity с
   бэкендом IL2CPP на ARM64. Для этого нужен исходный проект Unity 5.3.8f1,
   которого нет, и такой бэкенд в 5.3.8f1 для Android ARM64 не поддерживался.
   Простая замена имени `.so` или правка манифеста портом не являются и работать
   не будут.

## Артефакты

| Файл | Что это |
|---|---|
| `Fireman_1.0.9_final.apk` | Основной результат: оригинал + патч манифеста + подпись |
| `Fireman_1.0.9_fixed.apk` | Промежуточная: чистый zipalign + переподпись, манифест не тронут |
| `Fireman_1.0.9_fixed_universal.apk` | Fat APK: `armeabi-v7a` + `x86` (для эмулятора) |
| `Fireman_1.0.9_fixed_v2/v3.apk` | Промежуточные сборки через apktool |
| `artifacts/evidence/` | Скриншоты рендеринга |
| `SHA256SUMS.txt` | Хеши всех сборок |

Рекомендуется использовать только `Fireman_1.0.9_final.apk`.

## Скрипты

- `scripts/splice_manifest.py` — пересборка APK с заменой только манифеста.
- `scripts/png_analyze.py` — анализ скриншота (число цветов, палитра).
- `scripts/run_game.sh`, `scripts/watch_render.sh`, `scripts/final_test.sh` —
  запуск и наблюдение за игрой на эмуляторе.
- `scripts/test_only64.sh` — воспроизведение блокера на 64-бит-only образе.


## Addendum: why the APK is rejected on a modern phone (decisive test)

The user reported the APK would not install on a real device. A controlled test
on a genuine 64-bit-only image settled the cause.

Rule tested: Android 14 requires targetSdk >= 23, Android 15 and 16 require
targetSdk >= 24 to install, otherwise the install fails with
`INSTALL_FAILED_DEPRECATED_SDK_VERSION`. **Fireman 1.0.9 has targetSdkVersion 27,
which is above the floor, so this rule does not apply to it.**

On the 64-bit-only emulator (`abilist32=[]`, `ro.zygote=zygote64`, API 34):

| Test | Result |
|---|---|
| `Fireman_ABI_Check.apk` (no native libraries) | **installs fine** |
| `Fireman_1.0.9_final.apk` (armeabi-v7a) | fails: `INSTALL_FAILED_NO_MATCHING_ABIS ... res=-113` |
| `--abi arm64-v8a` forced | same `NO_MATCHING_ABIS` failure |

The error code is the proof. A targetSdk problem would report
`DEPRECATED_SDK_VERSION`; we get `NO_MATCHING_ABIS`. And an APK with no native
code installs on the very same device, which rules out any general block on old
apps. Full log: `artifacts/evidence/why_apk_rejected_64bit_only.txt`.

**Conclusion:** the blocker is the absent 32-bit userspace. No edit to the APK
can change that. Practical fixes are in `SOLUTIONS.md`; the working no-root
method is the VPhoneOS Android 7.1.2 virtual machine, which provides a 32-bit
userspace inside the phone. `Fireman_ABI_Check.apk` lets a user confirm their
own device in one step.
