let player;
let dotNetRef;

// Inicializa o YouTube Player API
function initYouTubePlayer(videoId, dotNetReference) {
    dotNetRef = dotNetReference;
    
    // Carrega a API do YouTube se ainda não foi carregada
    if (typeof YT === 'undefined') {
        const tag = document.createElement('script');
        tag.src = "https://www.youtube.com/iframe_api";
        const firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
        
        // Função global requerida pela API do YouTube
        window.onYouTubeIframeAPIReady = function() {
            createPlayer(videoId);
        };
    } else {
        createPlayer(videoId);
    }
}

function createPlayer(videoId) {
    // Remove player anterior se existir
    if (player) {
        player.destroy();
    }
    
    player = new YT.Player('youtube-player', {
        height: '100%',
        width: '100%',
        videoId: videoId,
        playerVars: {
            'autoplay': 1,
            'controls': 1,
            'modestbranding': 1,
            'rel': 0,
            'showinfo': 0
        },
        events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });
}

function onPlayerReady(event) {
    console.log('YouTube Player pronto');
    event.target.playVideo();
}

function onPlayerStateChange(event) {
    // YT.PlayerState.ENDED = 0
    if (event.data === YT.PlayerState.ENDED) {
        console.log('Vídeo terminou - avançando para próximo');
        // Notifica o Blazor que o vídeo terminou
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnVideoEnded');
        }
    }
    
    // Outros estados úteis:
    // YT.PlayerState.PLAYING = 1
    // YT.PlayerState.PAUSED = 2
    // YT.PlayerState.BUFFERING = 3
    // YT.PlayerState.CUED = 5
}

// Função para pausar o vídeo
function pauseVideo() {
    if (player && player.pauseVideo) {
        player.pauseVideo();
    }
}

// Função para retomar o vídeo
function playVideo() {
    if (player && player.playVideo) {
        player.playVideo();
    }
}

// Função para parar o vídeo
function stopVideo() {
    if (player && player.stopVideo) {
        player.stopVideo();
    }
}

// Função para obter a duração do vídeo
function getVideoDuration() {
    if (player && player.getDuration) {
        return player.getDuration();
    }
    return 0;
}

// Função para obter o tempo atual
function getCurrentTime() {
    if (player && player.getCurrentTime) {
        return player.getCurrentTime();
    }
    return 0;
}