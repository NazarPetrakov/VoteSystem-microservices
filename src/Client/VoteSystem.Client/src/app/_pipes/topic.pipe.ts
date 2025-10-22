import { Pipe, PipeTransform } from '@angular/core';
import { TopicGroups } from '../_models/_contracts/topic/topic';

@Pipe({
  name: 'topic',
})
export class TopicPipe implements PipeTransform {
  transform(topicValue: string, ...args: unknown[]): string {
    const topicGroup = TopicGroups;

    const allTopics = topicGroup.map((t) => t.topics).flat();

    const viewTopic =
      allTopics.find((t) => t.value === topicValue)?.viewValue ?? topicValue;

    return viewTopic;
  }
}
