(function () {
    'use strict';

    var triviaService = function ($http) {
        var triviaUrl = '/api/trivia',
            nextQuestion = function () {
                return $http.get(triviaUrl).then(function (response) {
                    return response.data;
                });
            },
            sendAnswer = function (option) {
                return $http.post(triviaUrl, { 'questionId': option.questionId, 'optionId': option.id }).then(function (response) {
                    return response.data;
                });
            };
        return {
            nextQuestion: nextQuestion,
            sendAnswer: sendAnswer
        };
    },

        module = angular.module('QuizApp');
    module.factory('triviaService', triviaService);

}());