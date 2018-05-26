![logo](https://github.com/GameEgg/STARPOO-II/blob/master/Resources/STARPOO-II-Logo.PNG)

## # STARPOO II 소개
게임개발 동아리 OOPARTS 5기 백종원/임경모가 팀으로 개발한 AI 대전 게임 2탄입니다.

STARPOO 라는 게임 이름 역시 동아리 이름 OOPARTS 에서 따온 것이죠! (와! 정말 유익한 정보입니다)

Javascript로 작성한 AI Script를 통해 자신의 함대를 조종하여 적 함선을 모두 격추시키는 것이 게임의 목표입니다. 


## # STARPOO II 의 룰
- 적 함선을 모두 격추시키면 승리합니다.
- 아군의 공격이 아군에게도 대미지를 줄 수 있습니다.
- 경기장의 경계를 넘어선 함선은 파괴됩니다.
- 일정 시간 이상 레이저가 함선에 히트하지 않으면, 경기장은 함선을 최소 1대 파괴시킬 때까지 축소됩니다!
- 함선은 일정 반경의 시야를 갖고 있고, AI는 이 시야 안의 적에 대한 정보만 알 수 있습니다.


## # STARPOO II 실행 방법
1. 먼저 최신 release를 받습니다. [https://github.com/GameEgg/STARPOO-II/releases](https://github.com/GameEgg/STARPOO-II/releases)  
2. 압축을 푼 뒤 실행파일이 있는 경로에 Script 폴더가 있는지 확인합니다. 
3. 게임을 더블클릭해 실행합니다.

![게임 실행 방법](https://github.com/GameEgg/STARPOO-II/blob/master/Resources/%ED%8F%B4%EB%8D%94.PNG)

* Script 폴더에 있는 js 파일들이 인공지능입니다. 인게임의 refresh 버튼을 통해, 게임을 종료하지 않고 스크립트를 갱신할 수 있습니다.


## # AI Script 작성 가이드
* [튜토리얼 1. 함선 움직여 보기 (1)](https://github.com/GameEgg/STARPOO-II/wiki/%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-1.-%ED%95%A8%EC%84%A0-%EC%9B%80%EC%A7%81%EC%97%AC-%EB%B3%B4%EA%B8%B0-(1))
* [튜토리얼 1. 함선 움직여 보기 (2)](https://github.com/GameEgg/STARPOO-II/wiki/%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-1.-%ED%95%A8%EC%84%A0-%EC%9B%80%EC%A7%81%EC%97%AC-%EB%B3%B4%EA%B8%B0-(2))  
* [튜토리얼 1. 함선 움직여 보기 (3)](https://github.com/GameEgg/STARPOO-II/wiki/%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-1.-%ED%95%A8%EC%84%A0-%EC%9B%80%EC%A7%81%EC%97%AC-%EB%B3%B4%EA%B8%B0-(3))  
* [튜토리얼 2. 적 함선에 레이저 발사해보기 (1)](https://github.com/GameEgg/STARPOO-II/wiki/%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-2.-%EC%A0%81-%ED%95%A8%EC%84%A0%EC%97%90-%EB%A0%88%EC%9D%B4%EC%A0%80-%EB%B0%9C%EC%82%AC%ED%95%B4%EB%B3%B4%EA%B8%B0-(1))  
* [튜토리얼 2. 적 함선에 레이저 발사해보기 (2)](https://github.com/GameEgg/STARPOO-II/wiki/%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-2.-%EC%A0%81-%ED%95%A8%EC%84%A0%EC%97%90-%EB%A0%88%EC%9D%B4%EC%A0%80-%EB%B0%9C%EC%82%AC%ED%95%B4%EB%B3%B4%EA%B8%B0-(2))  

## # [Script 에 제공되는 API](https://github.com/GameEgg/STARPOO-II/wiki/Script-API)

