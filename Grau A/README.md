# Tarefa: Grau A

## Equipe
- Allan Oliveira
- Felippe Carrion

## Comentários gerais

> Escolhemos a opção A: Movimentação de Objetos.

> O trabalho está sendo aplicado em um jogo ainda em desenvolvimento, alterando sistemas reais do projeto.

> O foco da aplicação é, a partir do espectro da música controlar: o movimento dos inimigos e o rastro de fumaça que o jogador deixa ao se movimentar.

> A entrega conterá uma build do projeto com as implementações e o autor do jogo (Allan Oliveira) irá enviar o projeto na Unity para conferir códigos e implementações de forma aprofundada.
  
## Comentários Explicativos

> No jogo, o player atira no tempo da música, e a precisão do tiro (Perfect, Good ou Miss) muda o dano. A música tem duas camadas: uma base e um solo de guitarra.
  •	Inimigos: andam em passos sincronizados com o beat. A força de cada passo e o pulso de tamanho vêm dos graves da mixagem.
  •	Jogador: deixa um rastro de fumaça enquanto anda. A quantidade de fumaça vem da energia do solo, e cada nota nova do solo solta um jato extra.
 As duas coisas se conectam pelo tiro: errar o tempo abaixa o volume do solo, o que reduz a fumaça na hora.

> Tecnologias Utilizadas:
  •	Unity (6000.4): Motor do jogo, inimigos, partículas.
  •	FMOD Studio (2.03): Montagem da música em camadas, tempo e parâmetros.
  •	FMOD for Unity - plugin (2.03): Reprodução, callback de beat e análise FFT em tempo real.

> Passo dos inimigos
O beat chega por callback do marcador de tempo do FMOD, e o horário de cada beat é registrado dentro do próprio callback. Com isso, cada inimigo calcula em que ponto do beat está.
Na prática, o inimigo dá um impulso no beat e desacelera até 30% da velocidade antes do próximo. Quanto mais grave no momento, mais forte o impulso. A trajetória continua sendo a de cada comportamento (perseguir, fugir, vagar, manter distância) o que muda é o ritmo com que ela é percorrida.

> Fumaça do jogador
A taxa de emissão por distância é proporcional à energia do solo.
Parado, o jogador não solta fumaça contínua, porque essa taxa só emite com o emissor em movimento. Com o solo em silêncio, E cai a zero e a fumaça some. Cada nota detectada soma um jato de 6 partículas, desde que o jogador esteja andando acima de 0,1 unidade por segundo.

> Interação com o tiro
No Miss, o jogo leva o parâmetro solo_volume do FMOD para 0,4 durante 0,3 s. A automação desse parâmetro abaixa o volume da track Solo. Como o FFT do solo fica depois desse volume, E cai junto e a fumaça diminui. O erro do jogador aparece ao mesmo tempo no som e na tela.
