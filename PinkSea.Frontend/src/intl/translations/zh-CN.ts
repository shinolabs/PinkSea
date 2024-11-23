export default {
  sidebar: {
    title: 'PinkSea',
    tag: '绘图留言版',
    shinolabs: '一个 shinonome laboratories 项目',
  },
  menu: {
    greeting: '您好 @{{name}}！',
    invitation: '登录以开始创作！',
    input_placeholder: '@alice.bsky.social',
    password: '密码（可选）',
    atp_login: '登录',
    my_oekaki: '投稿记录',
    recent: '最近投稿',
    settings: '设置',
    logout: '登出',
    create_something: '创建绘图',
    oauth2_info:
      '如果您在密码框处留空，PinkSea 将使用 OAuth2 登录您的 PDS。这比密码登录更安全。',
  },
  breadcrumb: {
    recent: '最近投稿',
    painter: '作者',
    settings: 'Pinksea 的设置',
    user_profile: '{{handle}} 的个人资料',
    user_post: '{{handle}} 的投稿',
    tagged: '打上 #{{tag}} 标签的投稿一览',
  },
  timeline: {
    by_before_handle: '作者 ',
    by_after_handle: '',
  },
  post: {
    response_from_before_handle: '',
    response_from_after_handle: ' 的回信，在 ',
    response_from_at_date: '', // in Chinese we don't use any words to seperate date and time
  },
  response_box: {
    login_to_respond: '登录以回信！',
    click_to_respond: '点击打开绘画面板',
    open_painter: '打开画板',
    reply: '回复！',
    cancel: '取消',
  },
  settings: {
    category_general: '通用',
    general_language: '语言',

    category_sensitive: '敏感媒体投稿',
    sensitive_blur_nsfw: '模糊 NSFW 帖文',
    sensitive_hide_nsfw: '不显示 NSFW 帖文',
  },
  painter: {
    do_you_want_to_restore: '之前的作品上传时发生了一个错误。要恢复已保存的作品吗？',
    could_not_send_post:
      '之前的作品上传时发生了一个错误。作品在浏览器内已保存，请稍后尝试投稿。',
    add_a_description: '这里记录画像的说明',
    tag: '标签',
    crosspost_to_bluesky: '交叉发布到 Bluesky',
    upload: '上传！',
  },
  profile: {
    bluesky_profile: 'Bluesky 个人资料',
    domain: '网站',
  },
  tegakijs: {
    // メッセージ
    badDimensions: '非法尺寸',
    promptWidth: '画布宽（像素）',
    promptHeight: '画布高（像素）',
    confirmDelLayers: '删除已选图层？',
    confirmMergeLayers: '合并选择图层？',
    tooManyLayers: '达到图层限制。',
    errorLoadImage: '无法加载图像。',
    noActiveLayer: '无活动图层。',
    hiddenActiveLayer: '活动图层不可见。',
    confirmCancel: '您确定吗？您的作品将会丢失。',
    confirmChangeCanvas: '您确定吗？更改画布将会清除所有图层、历史记录，并且禁用回放录制功能。',

    // コントロール
    color: '颜色',
    size: '尺寸',
    alpha: '透明度',
    flow: '流量',
    zoom: '放大/缩小',
    layers: '图层',
    switchPalette: '切换色板',
    paletteSlotReplace: '右键替换为当前颜色',

    // レイヤー
    layer: '图层',
    addLayer: '新建图层',
    delLayers: '删除图层',
    mergeLayers: '合并图层',
    moveLayerUp: '上移',
    moveLayerDown: '下移',
    toggleVisibility: '切换可见性',

    // メニューバー
    newCanvas: '新建',
    open: '打开',
    save: '保存',
    saveAs: '另存为',
    export: '导出',
    undo: '撤销',
    redo: '重做',
    close: '关闭',
    finish: '完成',

    // ツールモード
    tip: '笔尖',
    pressure: '笔压感知',
    preserveAlpha: '保持 Alpha 值',

    // ツール
    pen: '笔',
    pencil: '铅笔',
    airbrush: '喷枪',
    pipette: '取色器',
    blur: '模糊',
    eraser: '橡皮',
    bucket: '填充',
    tone: '调色',

    // リプレイ
    gapless: '无缝播放',
    play: '播放',
    pause: '暂停',
    rewind: '回退',
    slower: '慢速',
    faster: '快速',
    recordingEnabled: '正在录制回放',
    errorLoadReplay: '无法录制回放：',
    loadingReplay: '正在加载回放…',
  }
}
