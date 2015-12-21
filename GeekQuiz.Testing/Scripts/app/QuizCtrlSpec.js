/// <reference path="../jasmine-html.js" />
/// <reference path="../jasmine.js" />
/// <reference path="../../../geekquiz/scripts/angular.js" />
/// <reference path="../../../geekquiz/scripts/angular-mocks.js" />
/// <reference path="../../../geekquiz/scripts/app/app.js" />
/// <reference path="../../../geekquiz/scripts/app/quiz-controller.js" />
/// <reference path="../../../geekquiz/scripts/app/trivia-service.js" />

(function () {
    'use strict';

    describe('Controller: QuizCtrl', function () {
        var scope,
            options = [{
                id: 1,
                title: '1',
                questionId: 1
            }, {
                id: 2,
                title: '2',
                questionId: 1
            }],
            question = 'What is two times two?',
            data = { options: options, title: question };

        beforeEach(module('QuizApp'));

        beforeEach(inject(function ($rootScope, $controller, triviaService, $q) {
            var deferredPromiseResult = function (mockReturnValue) {
                var deferred = $q.defer();
                deferred.resolve(mockReturnValue);
                return deferred.promise;
            };

            scope = $rootScope.$new();

            spyOn(triviaService, 'nextQuestion').and.callFake(function () {
                return deferredPromiseResult(data);
            });
            spyOn(triviaService, 'sendAnswer').and.callFake(function () {
                return deferredPromiseResult(true);
            });
            $controller('QuizCtrl', {
                $scope: scope,
                triviaService: triviaService
            });
        }));

        it('should have scope defined', function () {
            expect(scope).toBeDefined();
        });

        it('should have expected scope values after calling nextQuestion', function () {
            scope.nextQuestion();
            // This is needed if scope object's value comes from promise result.
            scope.$root.$digest();
            expect(scope.options.length).toEqual(2);
            expect(scope.title).toEqual('What is two times two?');
        });

        it('should have expected scope values after calling sendAnswer', function () {
            var option = {};
            scope.sendAnswer(option);
            scope.$root.$digest();
            expect(scope.correctAnswer).toEqual(true);
        });
    });
}());