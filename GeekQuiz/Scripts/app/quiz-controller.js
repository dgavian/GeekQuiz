(function () {
    'use strict';

    var app = angular.module('QuizApp'),
        QuizCtrl = function ($scope, triviaService) {

            var defaultTitle = 'loading question...',
                onNextQuestionComplete = function (data) {
                    $scope.options = data.options;
                    $scope.title = data.title;
                    $scope.answered = false;
                    $scope.working = false;
                },
                onSendAnswerComplete = function (data) {
                    $scope.correctAnswer = (data === true);
                    $scope.working = false;
                },
                onError = function () {
                    $scope.title = 'Oops... something went wrong';
                    $scope.working = false;
                };

            $scope.answered = false;
            $scope.title = defaultTitle;
            $scope.options = [];
            $scope.correctAnswer = false;
            $scope.working = false;

            $scope.answer = function () {
                return $scope.correctAnswer ? 'correct' : 'incorrect';
            };

            $scope.nextQuestion = function () {
                $scope.working = true;
                $scope.answered = false;
                $scope.title = defaultTitle;
                $scope.options = [];

                triviaService.nextQuestion().then(onNextQuestionComplete, onError);
            };

            $scope.sendAnswer = function (option) {
                $scope.working = true;
                $scope.answered = true;

                triviaService.sendAnswer(option).then(onSendAnswerComplete, onError);
            };
        };

    app.controller('QuizCtrl', ['$scope', 'triviaService', QuizCtrl]);
}());