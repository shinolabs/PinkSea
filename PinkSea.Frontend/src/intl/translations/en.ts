export default {
  sidebar: {
    title: "PinkSea",
    tag: "oekaki BBS",
    shinolabs: "a shinonome laboratories project"
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
    oauth2_info: "If you leave the password field blank, PinkSea will use OAuth2 to log into your PDS. It is generally more secure than password login."
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
  },
  post: {
    response_from_before_handle: "Response from ",
    response_from_after_handle: "",
    response_from_at_date: " at ",
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
    crosspost_to_bluesky: "Cross-post to BlueSky",
    upload: "Upload!"
  },
  profile: {
    bluesky_profile: "Bluesky profile",
    domain: "Website"
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
    newCanvas: 'New',
    open: 'Open',
    save: 'Save',
    saveAs: 'Save As',
    export: 'Export',
    undo: 'Undo',
    redo: 'Redo',
    close: 'Close',
    finish: 'Finish',

    // Tool modes
    tip: 'Tip',
    pressure: 'Pressure',
    preserveAlpha: 'Preserve Alpha',

    // Tools
    pen: 'Pen',
    pencil: 'Pencil',
    airbrush: 'Airbrush',
    pipette: 'Pipette',
    blur: 'Blur',
    eraser: 'Eraser',
    bucket: 'Bucket',
    tone: 'Tone',

    // Replay
    gapless: 'Gapless',
    play: 'Play',
    pause: 'Pause',
    rewind: 'Rewind',
    slower: 'Slower',
    faster: 'Faster',
    recordingEnabled: 'Recording replay',
    errorLoadReplay: 'Could not load the replay: ',
    loadingReplay: 'Loading replay…',
  }
};
