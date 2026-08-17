using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class QuizAndCurriculumTests
    {
        [Fact]
        public void StudyGuideData_All19Topics_AreLoadedWithQuizzesAndFormulas()
        {
            var topics = StudyGuideData.GetTopics();
            topics.Should().HaveCount(19);

            foreach (var topic in topics)
            {
                topic.Title.Should().NotBeNullOrWhiteSpace();
                topic.Summary.Should().NotBeNullOrWhiteSpace();
                topic.MathFormulas.Should().NotBeNullOrWhiteSpace();
                topic.CodeSnippet.Should().NotBeNullOrWhiteSpace();
                topic.ComplexityAndTips.Should().NotBeNullOrWhiteSpace();
                topic.TargetLessonNumber.Should().BeInRange(1, 15);

                topic.Quiz.Should().NotBeNull();
                topic.Quiz!.Question.Should().NotBeNullOrWhiteSpace();
                topic.Quiz.Options.Should().HaveCount(3);
                topic.Quiz.CorrectOptionIndex.Should().BeInRange(0, 2);
                topic.Quiz.Explanation.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void InteractiveLabManager_All15Lessons_HaveStarterAndQuizzes()
        {
            var lessons = InteractiveLabManager.GetLessons();
            lessons.Should().HaveCount(15);

            foreach (var lesson in lessons)
            {
                lesson.Title.Should().NotBeNullOrWhiteSpace();
                lesson.Summary.Should().NotBeNullOrWhiteSpace();
                lesson.StarterTemplate.Should().NotBeNullOrWhiteSpace();
                lesson.ChallengeGoal.Should().NotBeNullOrWhiteSpace();

                lesson.QuizQuestion.Should().NotBeNullOrWhiteSpace();
                lesson.QuizOptions.Should().HaveCount(3);
                lesson.QuizOptions.Should().ContainSingle(o => o.IsCorrect);
            }
        }
    }
}
