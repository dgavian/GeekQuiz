/// <reference path="../jasmine-html.js" />
/// <reference path="../jasmine.js" />
/// <reference path="../../../geekquiz/scripts/angular.js" />
/// <reference path="../../../geekquiz/scripts/angular-mocks.js" />
/// <reference path="../../../geekquiz/scripts/app/app.js" />
/// <reference path="../../../geekquiz/scripts/app/quiz-controller.js" />
/// <reference path="../../../geekquiz/scripts/app/trivia-service.js" />

(function () {
    'use strict';

    describe('Service: triviaService', function () {
        var sut, backend,
            options = [{
                id: 1,
                title: '1',
                questionId: 1
            }, {
                id: 2,
                title: '2',
                questionId: 1
            }, {
                id: 3,
                title: '4',
                questionId: 1
            }, {
                id: 4,
                title: '8',
                questionId: 1
            }],
            question = 'What is two times four?',
            data = { options: options, title: question };

        beforeEach(module('QuizApp'));

        beforeEach(function () {
            inject(function ($httpBackend, _triviaService_) {
                sut = _triviaService_;
                backend = $httpBackend;
            });
        });

        afterEach(function () {
            backend.verifyNoOutstandingExpectation();
            backend.verifyNoOutstandingRequest();
        });

        it('should get expected data', function () {
            var returnedPromise, result;
            backend.expectGET('/api/trivia').respond(data);
            returnedPromise = sut.nextQuestion();
            returnedPromise.then(function (response) {
                result = response;
            });
            backend.flush();
            expect(result.options.length).toEqual(4);
            expect(result.title).toEqual('What is two times four?');
        });

        it('should post and return expected response', function () {
            var returnedPromise, result, option = {};
            backend.expectPOST('/api/trivia').respond(true);
            returnedPromise = sut.sendAnswer(option);
            returnedPromise.then(function (response) {
                result = response;
            });
            backend.flush();
            expect(result).toEqual(true);
        });
    });
}());