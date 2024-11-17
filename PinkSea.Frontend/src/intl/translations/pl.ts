export default {
  sidebar: {
    title: "PinkSea",
    tag: "oekaki BBS",
    shinolabs: "produkcja shinonome laboratories"
  },
  menu: {
    greeting: "Cześć @{{name}}!",
    invitation: "Zaloguj się aby tworzyć!",
    input_placeholder: "@alicja.bsky.social",
    atp_login: "@ Zaloguj",
    my_oekaki: "Moje oekaki",
    recent: "Ostatnie",
    settings: "Ustawienia",
    logout: "Wyloguj",
    create_something: "Stwórz nowe",
    password: "Hasło (opcjonalne)",
    oauth2_info: "Jeżeli zostawisz to pole puste, PinkSea użyje OAuth2 aby zalogować się do twojego PDSa. Jest to bezpieczniejsze."
  },
  breadcrumb: {
    recent: "ostatnie",
    painter: "rysowanie",
    settings: "twoje ustawienia",
    user_profile: "profil {{handle}}",
    user_post: "post {{handle}}",
    tagged: "posty otagowane #{{tag}}"
  },
  timeline: {
    by_before_handle: "Post ",
    by_after_handle: ""
  },
  post: {
    response_from_before_handle: "Odpowiedź ",
    response_from_after_handle: "",
    response_from_at_date: " dnia "
  },
  response_box: {
    login_to_respond: "Zaloguj się aby odpowiedzieć!",
    click_to_respond: "Kliknij przycisk aby otworzyć narzędzie do rysowania",
    open_painter: "Otwórz rysownik",
    reply: "Odpowiedz!",
    cancel: "Anuluj"
  },
  settings: {
    category_general: "ogólne",
    general_language: "Język",

    category_sensitive: "media",
    sensitive_blur_nsfw: "Zamazuj posty NSFW",
    sensitive_hide_nsfw: "Ukrywaj posty NSFW"
  },
  painter: {
    do_you_want_to_restore: "Ostatnia próba wysłania posta zakończyła się niepowodzeniem i zapisano Twoją ostatnią próbę. Czy ją otworzyć?",
    could_not_send_post: "Wystąpił błąd podczas wysyłania posta. Prosimy spróbować później. Twój post został zapisany w przeglądarce.",
    add_a_description: "Dodaj opis!",
    tag: "Tag",
    crosspost_to_bluesky: "Wyślij dodatkowo na BlueSky",
    upload: "Wyślij!"
  },
  profile: {
    bluesky_profile: "Profil Bluesky",
    domain: "Strona internetowa"
  },
  tegakijs: {
    // Messages
    badDimensions: 'Niepoprawny rozmiar.',
    promptWidth: 'Szerokość obrazu w pikselach',
    promptHeight: 'Wysokość obrazu w pikselach',
    confirmDelLayers: 'Usunąć wybrane warstwy?',
    confirmMergeLayers: 'Złączyć wybrane warstwy?',
    tooManyLayers: 'Osiągnięto limit warstw.',
    errorLoadImage: 'Napotkano błąd podczas ładowania zdjęcia.',
    noActiveLayer: 'Brak aktywnej warstwy.',
    hiddenActiveLayer: 'Aktywna warstwa jest ukrytwa.',
    confirmCancel: 'Czy zamknąć edytor? Zamknięcie edytora kasuje wszystkie zmiany.',
    confirmChangeCanvas: 'Czy zmienić obraz? Zmiana obrazu wyczyści wszystkie warstwy, historię oraz wyłączy odtwarzanie nagrań.',

    // Controls
    color: 'Kolor',
    size: 'Rozmiar',
    alpha: 'Przezroczystość',
    flow: 'Płynność',
    zoom: 'Powiększenie',
    layers: 'Warstwy',
    switchPalette: 'Zmień paletę kolorów',
    paletteSlotReplace: 'Kliknij prawym przyciskiem myszy aby zamienić z aktywnym kolorem',

    // Layers
    layer: 'Warstwa',
    addLayer: 'Dodaj warstwę',
    delLayers: 'Usuń warstwy',
    mergeLayers: 'Złącz warstwy',
    moveLayerUp: 'Przesuń w górę',
    moveLayerDown: 'Przesuń w dół',
    toggleVisibility: 'Zmień widoczność',

    // Menu bar
    newCanvas: 'Nowy',
    open: 'Otwórz',
    save: 'Zapisz',
    saveAs: 'Zapisz jako',
    export: 'Eksportuj',
    undo: 'Cofnij',
    redo: 'Powtórz',
    close: 'Zamknij',
    finish: 'Zatwierdź',

    // Tool modes
    tip: 'Końcówka',
    pressure: 'Nacisk',
    preserveAlpha: 'Zachowaj kanał alfa',

    // Tools
    pen: 'Pędzel',
    pencil: 'Ołówek',
    airbrush: 'Aerograf',
    pipette: 'Probówka',
    blur: 'Rozmycie',
    eraser: 'Gumka',
    bucket: 'Wiadro z farbą',
    tone: 'Ton',

    // Replay
    gapless: 'Bez przerw',
    play: 'Odtwórz',
    pause: 'Pauza',
    rewind: 'Przewiń',
    slower: 'Wolniej',
    faster: 'Szybciej',
    recordingEnabled: 'Nagrywanie odtwarzania',
    errorLoadReplay: 'Nie można odtworzyć nagrania: ',
    loadingReplay: 'Ładowanie nagrania…',
  }
};
