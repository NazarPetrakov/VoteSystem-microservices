export interface Poll {
  id: string;
  title: string;
  topic: string;
  userId: number;
  userName: string;
  isClosed: boolean;
  createdTime: Date;
  updatedTime: Date;
  pollOptions: PollOption[];
}
export interface PollOption {
  id: string;
  text: string;
  order: number;
  pollId: string;
  createdTime: Date;
  updatedTime?: Date;
}
export class CreatePollRequest {
  title: string;
  topic?: string;
  userId: number;
  isClosed: boolean;
  pollOptionsToCreate: CreatePollOptionFromPollDto[];

  constructor(
    title: string,
    userId: number,
    isClosed: boolean,
    pollOptionsToCreate: string[],
    topic?: string
  ) {
    const options = pollOptionsToCreate.map(
      (o) => new CreatePollOptionFromPollDto(o)
    );
    this.title = title;
    (this.topic = topic),
      (this.userId = userId),
      (this.isClosed = isClosed),
      (this.pollOptionsToCreate = options);
  }
}
class CreatePollOptionFromPollDto {
  text: string;
  constructor(text: string) {
    this.text = text;
  }
}
