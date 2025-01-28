export default {
  sidebar: {
    title: "PinkSea",
    tag: "oekaki BBS",
    shinolabs: "bir shinonome laboratuvarları projesi"
  },
  menu: {
    greeting: "Hi @{{name}}!",
    invitation: "Login to start creating!",
    input_placeholder: "@alice.bsky.social",
    password: "Password (Optional)",
    atp_login: "@ Login",
    my_oekaki: "My oekaki",
    recent: "Recent",
    settings: "Settings",
    logout: "Logout",
    create_something: "Create something",
    oauth2_info: "If you leave the password field blank, PinkSea will use OAuth2 to log into your PDS. It is generally more secure than password login.",
    search: "Search",
    search_placeholder: "Search for a tag",
    search_go: "Go"
  },
  breadcrumb: {
    recent: "recent",
    painter: "painter",
    settings: "your settings",
    user_profile: "{{handle}}'s profile",
    user_post: "{{handle}}'s post",
    tagged: "posts tagged #{{tag}}"
  },
  timeline: {
    by_before_handle: "By ",
    by_after_handle: "",
    nothing_here: "Nothing here so far... (╥﹏╥)"
  },
  post: {
    response_from_before_handle: "Response from ",
    response_from_after_handle: "",
    response_from_at_date: " at ",
    this_post_no_longer_exists: "This post no longer exists."
  },
  response_box: {
    login_to_respond: "Login to respond!",
    click_to_respond: "Click to open the drawing panel",
    open_painter: "Open painter",
    reply: "Reply!",
    cancel: "Cancel"
  },
  settings: {
    category_general: "general",
    general_language: "Language",

    category_sensitive: "sensitive media",
    sensitive_blur_nsfw: "Blur NSFW posts",
    sensitive_hide_nsfw: "Don't display NSFW posts"
  },
  painter: {
    do_you_want_to_restore: "The last upload has errored out and your image has been saved. Do you want to restore it?",
    could_not_send_post: "There was an issue uploading the post. Please try again later. Your post has been saved in your browser.",
    add_a_description: "Add a description!",
    tag: "Tag",
    crosspost_to_bluesky: "Cross-post to Bluesky",
    upload: "Upload!",
    upload_description: "Description",
    upload_tags: "Tags",
    upload_social: "Social",
    upload_confirm: "Confirm",
    hint_description: "Attaching a short description helps give context about your drawing. Optional.",
    hint_tags: "Give your post up to five tags to help others discover it! For example: characters (koiwai_yotsuba), copyrights (yotsubato! / oc) or general information (portrait). Optional.",
    hint_nsfw: "Please check if your post contains adult content such as nudity or highly suggestive themes.",
    hint_xpost: "If checked, we'll automatically create a post for you on Bluesky with the image and a link to PinkSea attached.",
    hint_confirm: "Once you're ready, click the button above to publish your image!"
  },
  profile: {
    bluesky_profile: "Bluesky profile",
    domain: "Website",
    posts_tab: "Posts",
    replies_tab: "Replies"
  },
  tegakijs: {
    // Messages
    badDimensions: 'Invalid dimensions.',
    promptWidth: 'Canvas width in pixels',
    promptHeight: 'Canvas height in pixels',
    confirmDelLayers: 'Delete selected layers?',
    confirmMergeLayers: 'Merge selected layers?',
    tooManyLayers: 'Layer limit reached.',
    errorLoadImage: 'Could not load the image.',
    noActiveLayer: 'No active layer.',
    hiddenActiveLayer: 'The active layer is not visible.',
    confirmCancel: 'Are you sure? Your work will be lost.',
    confirmChangeCanvas: 'Are you sure? Changing the canvas will clear all layers and history and disable replay recording.',

    // Controls
    color: 'Color',
    size: 'Size',
    alpha: 'Opacity',
    flow: 'Flow',
    zoom: 'Zoom',
    layers: 'Layers',
    switchPalette: 'Switch color palette',
    paletteSlotReplace: 'Right click to replace with the current color',

    // Layers
    layer: 'Layer',
    addLayer: 'Add layer',
    delLayers: 'Delete layers',
    mergeLayers: 'Merge layers',
    moveLayerUp: 'Move up',
    moveLayerDown: 'Move down',
    toggleVisibility: 'Toggle visibility',

    // Menu bar
    newCanvas: 'Yeni',
    open: 'Aç',
    save: 'Kaydet',
    saveAs: 'Farklı Kaydet',
    export: 'Export', // todo
    undo: 'Geri al',
    redo: 'İleri al',
    close: 'Kapat',
    finish: 'Tamamla',

    // Tool modes
    tip: 'Tip', // i forgor
    pressure: 'Basınç',
    preserveAlpha: 'Alpha kanalını koru',

    // Tools
    pen: 'Kalem',
    pencil: 'Kurşun kalem',
    airbrush: 'Airbrush', // ?
    pipette: 'Pipet',
    blur: 'Buğula',
    eraser: 'Silgi',
    bucket: 'Kova',
    tone: 'Ton',

    // Replay
    gapless: 'Aralıksız',
    play: 'Oynat',
    pause: 'Duraklat',
    rewind: 'Geri oynat',
    slower: 'Daha yavaş',
    faster: 'Daha hızlı',
    recordingEnabled: 'Replay kaydediliyor', // replay?
    errorLoadReplay: 'Replay açılamadı: ',
    loadingReplay: 'Replay yükleniyor…',
  }
};
